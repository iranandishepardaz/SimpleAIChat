using ApcoAgentCore.Models;
using ApcoAgentCore.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace SimpleAIChat
{
    /// <summary>
    /// صفحه‌ی چت اصلی (نسخه ۰۵).
    /// 
    /// معماری کلی:
    ///   [کاربر] → txtQuestion
    ///        ↓
    ///   [ChatPage05] → _chatHistory + TrimHistory + BuildRequest
    ///        ↓
    ///   [LLMService] → ارسال به مدل و دریافت پاسخ
    ///        ↓
    ///   [ChatPage05] → نمایش پیام و آمار
    /// 
    /// مفاهیم کلیدی:
    ///   - Preset: رفتار مدل (system prompt، knowledge، temperature و…)
    ///   - LLM: موتور (endpoint، api key، context window، …)
    ///   - ChatRequest: بسته‌ی کامل ارسال (مدل + پیام‌ها + پارامترها)
    ///   - TrimHistory: مدیریت Context Window با حذف پیام‌های قدیمی
    /// 
    /// حالت‌های دیباگ:
    ///   - chkDebug: پنل دیباگ و پیام‌های مهم (Normal) را روشن می‌کند.
    ///   - chkVerbose: لاگ‌های پرحجم (chunkها، SSE خام) را در یک فایل جدا
    ///     ذخیره می‌کند تا txtDebug شلوغ نشود.
    /// </summary>
    public partial class ChatPage05 : Form
    {
        // ---------------------------------------------------------
        // فیلدها
        // ---------------------------------------------------------

        /// <summary>سرویس ارتباط با مدل‌های زبانی.</summary>
        private readonly LLMService _llmService = new LLMService();

        /// <summary>تاریخچه‌ی کامل گفتگو (user و assistant). system‌ها اینجا نیستند.</summary>
        private readonly List<ChatMessage> _chatHistory = new List<ChatMessage>();

        /// <summary>همه‌ی مدل‌های فعال، بارگذاری‌شده از models.json.</summary>
        private List<LLM> _models = new List<LLM>();

        /// <summary>مدل انتخاب‌شده در ComboBox.</summary>
        private LLM _selectedModel;

        /// <summary>همه‌ی پریست‌ها، بارگذاری‌شده از presets.json.</summary>
        private List<Preset> _presets = new List<Preset>();

        /// <summary>پریست انتخاب‌شده در ComboBox.</summary>
        private Preset _selectedPreset;

        /// <summary>آخرین پاسخ مدل (خام یا JSON‌شده‌ی تمیز).</summary>
        private string _lastAnswer = "";

        /// <summary>برای لغو پاسخ در حال دریافت.</summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>مجموع توکن مصرفی از ابتدای گفتگو (برای نمایش به کاربر).</summary>
        private int _totalConversationTokens = 0;

        // لاگر داخلی (Normal)
        private readonly StringBuilder _debugLog = new StringBuilder();

        // لاگر فایل (Verbose) — فقط وقتی chkDebug و chkVerbose روشن‌اند.
        private readonly StringBuilder _chunkLog = new StringBuilder();
        private string _chunkLogPath;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer timerHide;

        // ---------------------------------------------------------
        // سازنده
        // ---------------------------------------------------------
        public ChatPage05()
        {
            InitializeComponent();

            // ترتیب مهم است: ابتدا پریست‌ها، بعد مدل‌ها.
            LoadPresets();
            LoadModels();

            _chatHistory.Clear();

            toolTip1 = new System.Windows.Forms.ToolTip();

            timerHide = new System.Windows.Forms.Timer();
            timerHide.Interval = 3000;
            timerHide.Tick += timerHide_Tick;
        }

        // ---------------------------------------------------------
        // بارگذاری مدل‌ها از سرویس
        // ---------------------------------------------------------
        private void LoadModels()
        {
            try
            {
                // فقط مدل‌های Enabled
                _models = LLMService.LoadModels(LoadAll: false);

                cmbModel.DataSource = null;
                cmbModel.DataSource = _models;
                cmbModel.DisplayMember = "Id";
                cmbModel.ValueMember = "Id";

                if (_models.Count > 0)
                    cmbModel.SelectedIndex = 0;

                lblStatus.Text = $"Models loaded ({_models.Count}).";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading models.";
                MessageBox.Show("خطا در خواندن فایل مدل‌ها:\r\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------------------------------------------------------
        // بارگذاری پریست‌ها از سرویس
        // ---------------------------------------------------------
        private void LoadPresets()
        {
            _presets = PresetService.LoadPresets();

            cmbPreset.DataSource = null;
            cmbPreset.DataSource = _presets;
            cmbPreset.DisplayMember = "Name";
            cmbPreset.ValueMember = "Id";

            if (_presets.Count > 0)
                cmbPreset.SelectedIndex = 0;
            else
                _selectedPreset = null;
        }

        // ---------------------------------------------------------
        // ارسال سؤال (قلب فرم)
        // ---------------------------------------------------------
        private async void btnSend_Click(object sender, EventArgs e)
        {
            // ۱) اعتبارسنجی اولیه
            if (_selectedModel == null)
            {
                MessageBox.Show("لطفاً یک مدل انتخاب کنید.");
                return;
            }

            string question = txtQuestion.Text.Trim();
            string summary = "";

            if (string.IsNullOrWhiteSpace(question))
            {
                MessageBox.Show("لطفاً یک سؤال وارد کنید.");
                return;
            }

            // ۲) آماده‌سازی UI
            StartAnswering();

            // ۳) افزودن پیام کاربر به تاریخچه و UI
            _chatHistory.Add(new ChatMessage { Role = "user", Content = question });
            AddMessageBox(question, summary, isAnswer: false);

            txtQuestion.Clear();
            txtQuestion.Focus();

            try
            {
                // ۴) Trim تاریخچه بر اساس Context Window مدل
                List<ChatMessage> history = Utilities.TrimHistory(
                    _chatHistory, _selectedModel, _selectedPreset);

                if (Utilities.LastTrimmedCount > 0)
                    LogDebug($"[trim] removed {Utilities.LastTrimmedCount} message(s)");

                // ۵) ساخت درخواست کامل (system + knowledge + history)
                ChatRequest request = LLMService.BuildRequest(
                    _selectedModel, _selectedPreset, history);
                request.UseStream = chkStream.Checked;

                DateTime startTime = DateTime.Now;

                // سطح لاگ: Normal یا Verbose
                DebugLevel level = DebugLevel.Normal;
                if (chkDebug.Checked && chkVerbose.Checked)
                    level = DebugLevel.Verbose;

                // ۶) ارسال به مدل
                string answer = await _llmService.SendMessageAsync(
                    request,
                    onChunkReceived: chkStream.Checked ? OnStreamChunk : null,
                    onDebugLog: chkDebug.Checked ? LogDebug : null,
                    onVerboseLog: LogVerboseToFile,
                    debugLevel: level,
                    cancellationToken: _cancellationTokenSource.Token);

                _lastAnswer = answer;

                // ۷) اگر پریست JSON بود، پاسخ را parse کن
                if (_selectedPreset != null &&
                    string.Equals(_selectedPreset.OutputFormat, "json", StringComparison.OrdinalIgnoreCase))
                {
                    Dictionary<string, string> parsed =Utilities. TryParseJsonAnswer(answer, out string jsonError);

                    if (parsed != null && parsed.Count > 0)
                    {
                        StringBuilder pretty = new StringBuilder();
                        pretty.AppendLine("✅ JSON parsed successfully:");
                        pretty.AppendLine();
                        foreach (var kv in parsed)
                            pretty.AppendLine($"{kv.Key}: {kv.Value}");

                        _lastAnswer = pretty.ToString();
                        LogDebug($"[json] parsed {parsed.Count} field(s)");
                    }
                    else
                    {
                        _lastAnswer = "⚠️ JSON parse failed: " + (jsonError ?? "unknown error") +
                                      "\r\n\r\nRaw response:\r\n" + answer;
                        LogDebug($"[json] parse failed: {jsonError}");
                    }
                }

                // ۸) ساخت dump برای دیباگ (فقط در حافظه، نه فایل)
                string dump = Utilities.BuildDump(
                    _selectedModel, _selectedPreset,
                    _chatHistory, history, _lastAnswer);
                LogDebug(dump);

                // ۹) محاسبه‌ی توکن‌ها (usage واقعی یا تخمین)
                int promptTokens, completionTokens, totalTokens;

                if (request.LastPromptTokens.HasValue && request.LastCompletionTokens.HasValue)
                {
                    promptTokens = request.LastPromptTokens.Value;
                    completionTokens = request.LastCompletionTokens.Value;
                }
                else
                {
                    string promptText = string.Join("\n", request.Messages.Select(m => m.Content));
                    promptTokens = Utilities.EstimateTokens(promptText);
                    completionTokens = Utilities.EstimateTokens(answer);
                }

                totalTokens = promptTokens + completionTokens;

                // ۱۰) جمع کل گفتگو
                _totalConversationTokens += totalTokens;

                // ۱۱) خلاصه‌ی بالای پاسخ
                TimeSpan elapsed = DateTime.Now - startTime;
                summary =
                    $"Done | Tokens: {totalTokens} | " +
                    $"Prompt: {promptTokens} | " +
                    $"Answer: {completionTokens} | " +
                    $"Time: {elapsed.TotalSeconds:0.0}s | " +
                    $"Total: {_totalConversationTokens}";

                // ۱۲) نمایش پاسخ
                if (chkStream.Checked)
                    FinalizeStreamingMessage(_lastAnswer, summary);
                else
                    AddMessageBox(_lastAnswer, summary, isAnswer: true);

                // ۱۳) افزودن پاسخ به تاریخچه (خام، نه JSON تمیزشده)
                _chatHistory.Add(new ChatMessage { Role = "assistant", Content = answer });

                lblStatus.Text = summary;
            }
            catch (OperationCanceledException)
            {
                lblStatus.Text = "Response cancelled.";
                RemoveStreamingBoxIfAny();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
                RemoveStreamingBoxIfAny();
                MessageBox.Show("خطا در ارتباط با مدل: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                StopAnswering();
            }
        }

          // ---------------------------------------------------------
        // مدیریت Stream
        // ---------------------------------------------------------
        private TextBox _streamingTextBox;

        private void OnStreamChunk(string fullText)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(OnStreamChunk), fullText);
                return;
            }

            if (!string.IsNullOrEmpty(lblStatus.Text) && lblStatus.Text.Length > 23)
                lblStatus.Text = lblStatus.Text.Substring(0, 23) +
                                 DateTime.Now.ToString(" till now HH:mm:ss");

            if (_streamingTextBox == null || _streamingTextBox.IsDisposed)
                _streamingTextBox = AddStreamingMessageBox();

            _streamingTextBox.Text = fullText.Replace("\n", "\r\n");
            _streamingTextBox.SelectionStart = _streamingTextBox.Text.Length;
            _streamingTextBox.ScrollToCaret();
        }

        private void FinalizeStreamingMessage(string finalAnswer, string summary)
        {
            if (_streamingTextBox != null && !_streamingTextBox.IsDisposed)
            {
                int index = pnlConversation.Controls.IndexOf(_streamingTextBox);
                if (index >= 0)
                    pnlConversation.Controls.RemoveAt(index);

                _streamingTextBox.Dispose();
                _streamingTextBox = null;

                RecalculateLayout();
            }

            AddMessageBox(finalAnswer, summary, isAnswer: true);
        }

        private void RemoveStreamingBoxIfAny()
        {
            if (_streamingTextBox != null && !_streamingTextBox.IsDisposed)
            {
                pnlConversation.Controls.Remove(_streamingTextBox);
                _streamingTextBox.Dispose();
                _streamingTextBox = null;
                RecalculateLayout();
            }
        }

        // ---------------------------------------------------------
        // شروع/پایان حالت پاسخ‌دهی
        // ---------------------------------------------------------
        private void StartAnswering()
        {
            lblStatus.Text = $"Answering...   {DateTime.Now.ToString("HH:mm:ss")}";

            btnSend.Visible = false;
            btnCancell.Visible = true;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void StopAnswering()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            btnSend.Visible = true;
            btnCancell.Visible = false;
            txtQuestion.Focus();
        }

        // ---------------------------------------------------------
        // ساخت پیام عادی در Panel
        // ---------------------------------------------------------
        private TextBox AddMessageBox(string message, string summary, bool isAnswer)
        {
            int width = pnlConversation.ClientSize.Width - 25;

            Panel container = new Panel
            {
                Width = width,
                Left = 0,
                Height = 0,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };

            Label title = new Label
            {
                Text = isAnswer ? (_selectedModel?.Name ?? "Assistant") + " " + summary : "User",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = isAnswer ? Color.Yellow : Color.White,
                Width = width - 20,
                Top = 0,
                Height = 25,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleCenter
            };

            TextBox textBox = new TextBox
            {
                Text = string.IsNullOrWhiteSpace(message)
                    ? (isAnswer ? "پاسخی دریافت نشد." : "(پیام خالی)")
                    : message.Replace("\n", "\r\n"),
                Multiline = true,
                ReadOnly = true,
                RightToLeft = RightToLeft,
                TextAlign = HorizontalAlignment.Right,
                Width = width - 50,
                Left = isAnswer ? 15 : 35,
                Top = 28,
                ScrollBars = ScrollBars.None,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = isAnswer ? Color.LightYellow : Color.LightBlue,
                Font = new Font("Tahoma", 10, FontStyle.Regular),
                Padding = new Padding(5),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            textBox.Height = CalculateMessageHeight(textBox);
            container.Height = textBox.Top + textBox.Height + 10;

            container.Controls.Add(title);
            container.Controls.Add(textBox);

            pnlConversation.Controls.Add(container);

            RecalculateLayout();
            pnlConversation.ScrollControlIntoView(container);

            return textBox;
        }

        private TextBox AddStreamingMessageBox()
        {
            TextBox textBox = new TextBox
            {
                Text = "⏳ در حال دریافت پاسخ...\r\n\r\n",
                Multiline = true,
                ReadOnly = true,
                RightToLeft = RightToLeft,
                TextAlign = HorizontalAlignment.Right,
                Height = 150,
                Width = pnlConversation.ClientSize.Width - 50,
                Left = 15,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.LightYellow,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Tahoma", 10, FontStyle.Regular),
                Padding = new Padding(5)
            };

            pnlConversation.Controls.Add(textBox);

            RecalculateLayout();
            pnlConversation.ScrollControlIntoView(textBox);

            return textBox;
        }

        private int CalculateMessageHeight(TextBox textBox)
        {
            int width = textBox.ClientSize.Width - 8;
            if (width < 1)
                width = 1;

            using (Graphics graphics = textBox.CreateGraphics())
            using (StringFormat format = new StringFormat())
            {
                format.FormatFlags = StringFormatFlags.LineLimit;

                SizeF size = graphics.MeasureString(
                    textBox.Text,
                    textBox.Font,
                    width,
                    format);

                int height = (int)Math.Ceiling(size.Height) + 12;
                return Math.Max(height, 40);
            }
        }

        private void RecalculateLayout()
        {
            pnlConversation.AutoScrollPosition = new Point(0, 0);

            int top = 0;
            foreach (Control control in pnlConversation.Controls)
            {
                control.Top = top;
                top += control.Height + 5;
            }
        }

        // ---------------------------------------------------------
        // گفتگوی جدید
        // ---------------------------------------------------------
        private void btnNewChat_Click(object sender, EventArgs e)
        {
            FlushChunkLog();

            _chatHistory.Clear();
            pnlConversation.Controls.Clear();
            _streamingTextBox = null;
            _totalConversationTokens = 0;
            _lastAnswer = "";

            lblStatus.Text = "New conversation started.";
            txtQuestion.Clear();
            txtQuestion.Focus();
        }

        // ---------------------------------------------------------
        // لغو پاسخ در حال دریافت
        // ---------------------------------------------------------
        private void btnCancell_Click(object sender, EventArgs e)
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                lblStatus.Text = "Cancelling...";
            }
        }

        // ---------------------------------------------------------
        // تغییر مدل و پریست
        // ---------------------------------------------------------
        private void cmbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedModel = cmbModel.SelectedItem as LLM;

            if (_selectedModel == null)
                return;

            lblStatus.Text = "Selected Model : " + _selectedModel.Provider + " > " + _selectedModel.Name;
            lblLLMRemarks.Text = (_selectedModel.UseProxy ? " (Proxied!)" : "") + _selectedModel.Remarks;
        }

        private void cmbPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedPreset = cmbPreset.SelectedItem as Preset;

            if (_selectedPreset == null)
                return;

            lblStatus.Text = "Preset: " + _selectedPreset.Name;
        }

        // ---------------------------------------------------------
        // کپی Status با دوبار کلیک
        // ---------------------------------------------------------
        private void lblStatus_DoubleClick(object sender, EventArgs e)
        {
            if (sender is Label label && !string.IsNullOrEmpty(label.Text))
            {
                Clipboard.SetText(label.Text);
                MessageBox.Show("copied to clipboard");
            }
        }

        // ---------------------------------------------------------
        // پنل Debug و سطح‌های لاگ
        // ---------------------------------------------------------
        private void chkDebug_CheckedChanged(object sender, EventArgs e)
        {
            pnlDebug.Visible = chkDebug.Checked;

            if (chkDebug.Checked)
            {
                this.Height += pnlDebug.Height;
                pnlConversation.Height -= pnlDebug.Height;
            }
            else
            {
                this.Height -= pnlDebug.Height;
                pnlConversation.Height += pnlDebug.Height;
            }

            // chkVerbose فقط وقتی معنا دارد که debug روشن باشد
            chkVerbose.Enabled = chkDebug.Checked;

            if (chkDebug.Checked)
                LogDebug($"[debug] enabled at {DateTime.Now:HH:mm:ss}");
        }

        private void chkVerbose_CheckedChanged(object sender, EventArgs e)
        {
            LogDebug($"[verbose] {(chkVerbose.Checked ? "enabled" : "disabled")}");
        }

        /// <summary>
        /// پیام‌های Normal. اگر chkDebug خاموش باشد، هزینه صفر است.
        /// </summary>
        private void LogDebug(string message)
        {
            if (!chkDebug.Checked) return;

            _debugLog.AppendLine(message);

            // جلوگیری از رشد بی‌نهایت
            if (_debugLog.Length > 200_000)
                _debugLog.Remove(0, 100_000);

            if (txtDebug.InvokeRequired)
                txtDebug.Invoke(new Action(RefreshDebugBox));
            else
                RefreshDebugBox();
        }

        private void RefreshDebugBox()
        {
            txtDebug.Text = _debugLog.ToString();
            txtDebug.SelectionStart = txtDebug.Text.Length;
            txtDebug.ScrollToCaret();
        }

        /// <summary>
        /// لاگ‌های Verbose (chunkها و SSE خام) را در یک فایل جدا می‌نویسد.
        /// تا txtDebug فقط پیام‌های مهم را نگه دارد.
        /// </summary>
        private void LogVerboseToFile(string message)
        {
            if (!chkDebug.Checked || !chkVerbose.Checked) return;

            if (_chunkLogPath == null)
            {
                string folder = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "ApcoAgentCore", "logs");

                System.IO.Directory.CreateDirectory(folder);

                _chunkLogPath = System.IO.Path.Combine(
                    folder,
                    $"chunks-{DateTime.Now:yyyyMMdd-HHmmss}.log");

                System.IO.File.WriteAllText(_chunkLogPath,
                    $"[verbose log started at {DateTime.Now:HH:mm:ss}]\r\n");

                LogDebug($"[verbose] chunk log file: {_chunkLogPath}");
            }

            _chunkLog.AppendLine(message);

            // هر ۴۰۰۰ کاراکتر، به فایل بنویس تا حافظه سبک بماند
            if (_chunkLog.Length > 4000)
            {
                System.IO.File.AppendAllText(_chunkLogPath, _chunkLog.ToString());
                _chunkLog.Clear();
            }
        }

        /// <summary>
        /// باقی‌مانده‌ی chunk log را در فایل می‌ریزد.
        /// </summary>
        private void FlushChunkLog()
        {
            if (_chunkLogPath != null && _chunkLog.Length > 0)
            {
                System.IO.File.AppendAllText(_chunkLogPath, _chunkLog.ToString());
                _chunkLog.Clear();
            }

            _chunkLogPath = null;
        }

        // ---------------------------------------------------------
        // ناوبری History (بین صفحات قدیمی)
        // ---------------------------------------------------------
        private void cmbHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbHistory.SelectedIndex)
            {
                case 0: new ChatPage1().ShowDialog(); break;
                case 1: new ChatPage00().ShowDialog(); break;
                case 2: new ChatPage01().ShowDialog(); break;
                case 3: new ChatPage02().ShowDialog(); break;
                case 4: new ChatPage03().ShowDialog(); break;
                case 5: new ChatPage04().ShowDialog(); break;
            }
        }

        // ---------------------------------------------------------
        // ابزارهای Debug
        // ---------------------------------------------------------
        private void btnClearDebug_Click(object sender, EventArgs e)
        {
            _debugLog.Clear();
            txtDebug.Clear();
        }

        private void btnCopyLog_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(txtDebug.Text);
                ShowTempMessage("در کلیپ‌بورد کپی شد");
            }
            catch
            {
                ShowTempMessage("خطا! کپی کلیپ‌بورد انجام نشد!!");
            }
        }

        private void btnSaveLog_Click(object sender, EventArgs e)
        {
            string savedPath = LogService.SaveText(txtDebug.Text);

            if (savedPath.Length > 0)
            {
                DialogResult result = MessageBox.Show(
                    "لاگ ذخیره شد:\n" + savedPath + "\n\nفایل باز شود؟",
                    "موفق",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                    Process.Start("notepad.exe", savedPath);
            }
            else
            {
                MessageBox.Show("ذخیره لاگ انجام نشد.", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowTempMessage(string message)
        {
            toolTip1.Show(message, btnCopyLog, btnCopyLog.Width / 2, btnCopyLog.Height);
            timerHide.Stop();
            timerHide.Start();
        }

        private void timerHide_Tick(object sender, EventArgs e)
        {
            timerHide.Stop();
            toolTip1.Hide(this);
        }

        // ---------------------------------------------------------
        // باز کردن فرم تنظیمات
        // ---------------------------------------------------------
        private void btnSettings_Click(object sender, EventArgs e)
        {
            new SettingsForm().ShowDialog();

            int presetIndex = cmbPreset.SelectedIndex;
            int modelIndex = cmbModel.SelectedIndex;

            LoadPresets();
            if (presetIndex < cmbPreset.Items.Count)
                cmbPreset.SelectedIndex = presetIndex;

            LoadModels();
            if (modelIndex < cmbModel.Items.Count)
                cmbModel.SelectedIndex = modelIndex;
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            if (_selectedModel != null)
            {
                string info = _llmService.GetModelInfo(_selectedModel);
                MessageBox.Show(info, "Model Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}