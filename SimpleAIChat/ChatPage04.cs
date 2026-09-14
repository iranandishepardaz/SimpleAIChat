using ApcoAgentCore.Models;
using ApcoAgentCore.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleAIChat
{
    public partial class ChatPage04 : Form
    {
        // ---------------------------------------------------------
        // فیلدها
        // ---------------------------------------------------------
        private readonly LLMService00 _llmService = new LLMService00();
        private readonly List<ChatMessage> _chatHistory = new List<ChatMessage>();
        private List<LLM> _models = new List<LLM>();

        private LLM _selectedModel;
        private CancellationTokenSource _cancellationTokenSource;

        // لاگر داخلی
        private readonly StringBuilder _debugLog = new StringBuilder();
        private readonly Stopwatch _stopwatch = new Stopwatch();


        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer timerHide;


        public ChatPage04()
        {
            InitializeComponent();
            LoadModels();

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
                _models = LLMService00.LoadModels(LoadAll: false); // فقط Enabled

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

            _chatHistory.Clear();
        }

        // ---------------------------------------------------------
        // ارسال سؤال
        // ---------------------------------------------------------
        private async void btnSend_Click(object sender, EventArgs e)
        {
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

            StartAnswering();

            _chatHistory.Add(new ChatMessage { Role = "user", Content = question });
            AddMessageBox(question, summary, isAnswer: false);

            txtQuestion.Clear();
            txtQuestion.Focus();

            try
            {
                string prompt = BuildPrompt();
                DateTime startTime = DateTime.Now;

                string answer;
                int promptTokens = 0, completionTokens = 0, totalTokens = 0;

                if (chkStream.Checked)
                {
                    // برای Stream باید prompt را کامل بفرستیم و chunk ها را دریافت کنیم
                    answer = await _llmService.SendMessageAsync(
                        _selectedModel,
                        prompt,
                        useStream: true,
                        onChunkReceived: OnStreamChunk,
                         onDebugLog: chkDebug.Checked ? LogDebug : null,
                        cancellationToken: _cancellationTokenSource.Token);

                    // تخمین توکن‌ها برای Stream (چون API معمولاً usage نمی‌دهد)
                    promptTokens = EstimateTokens(prompt);
                    completionTokens = EstimateTokens(answer);
                    totalTokens = promptTokens + completionTokens;
                }
                else
                {
                    answer = await _llmService.SendMessageAsync(
                        _selectedModel,
                        prompt,
                        useStream: false,
                        onChunkReceived: null,
                        onDebugLog: chkDebug.Checked ? LogDebug : null,
                        cancellationToken: _cancellationTokenSource.Token);

                    // حداقل تخمین توکن
                    promptTokens = EstimateTokens(prompt);
                    completionTokens = EstimateTokens(answer);
                    totalTokens = promptTokens + completionTokens;
                }

                TimeSpan elapsed = DateTime.Now - startTime;
                summary = 
                    $"Done | Tokens: {totalTokens} | " +
                    $"Prompt: {promptTokens} | " +
                    $"Answer: {completionTokens} | " +
                    $"Time: {elapsed.TotalSeconds:0.0}s";
                // اگر Stream بود، TextBox موقت را حذف و پاسخ نهایی را اضافه می‌کنیم
                if (chkStream.Checked)
                {
                    FinalizeStreamingMessage(answer, summary);
                }
                else
                {
                    AddMessageBox(answer, summary, isAnswer: true);
                }

                _chatHistory.Add(new ChatMessage { Role = "assistant", Content = answer });

                lblStatus.Text =summary;
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
        // ساخت prompt از کل تاریخچه گفتگو
        // ---------------------------------------------------------
        private string BuildPrompt()
        {
            StringBuilder sb = new StringBuilder();

            foreach (ChatMessage msg in _chatHistory)
            {
                if (msg.Role == "user")
                    sb.AppendLine("User: " + msg.Content);
                else
                    sb.AppendLine("Assistant: " + msg.Content);
            }

            sb.AppendLine("Assistant:"); // مدل ادامه بدهد
            return sb.ToString();
        }

        // ---------------------------------------------------------
        // تخمین تعداد توکن (تقریبی - 4 کاراکتر = 1 توکن)
        // ---------------------------------------------------------
        private int EstimateTokens(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            return Math.Max(1, text.Length / 4);
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

            if (_streamingTextBox == null || _streamingTextBox.IsDisposed)
            {
                _streamingTextBox = AddStreamingMessageBox();
            }

            _streamingTextBox.Text = fullText.Replace("\n", "\r\n");
            _streamingTextBox.SelectionStart = _streamingTextBox.Text.Length;
            _streamingTextBox.ScrollToCaret();
        }

        private void FinalizeStreamingMessage(string finalAnswer , string summary)
        {
            if (_streamingTextBox != null && !_streamingTextBox.IsDisposed)
            {
                // جایگزینی TextBox موقت با پیام نهایی
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
        // شروع حالت دریافت پاسخ
        // ---------------------------------------------------------
        private void StartAnswering()
        {
            lblStatus.Text = "Answering...";

            btnSend.Visible = false;
            btnCancell.Visible = true;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        // ---------------------------------------------------------
        // پایان حالت دریافت پاسخ
        // ---------------------------------------------------------
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
        private TextBox AddMessageBox(string message,string summary, bool isAnswer)
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
                Text = isAnswer ? (_selectedModel?.Name ?? "Assistant") + " "+ summary : "User",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = isAnswer ? Color.Yellow : Color.White,
                Width = width - 20,
                Top = 0,
                Height = 25,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                TextAlign =  ContentAlignment.MiddleCenter
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

        // ---------------------------------------------------------
        // TextBox موقت برای پاسخ Stream
        // ---------------------------------------------------------
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

        // ---------------------------------------------------------
        // محاسبه ارتفاع مناسب پیام
        // ---------------------------------------------------------
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

        // ---------------------------------------------------------
        // چیدمان پیام‌ها
        // ---------------------------------------------------------
        private void RecalculateLayout()
        {
            // ریست اسکرول تا Topها نسبت به مبدأ واقعی محاسبه شوند
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
            _chatHistory.Clear();
            pnlConversation.Controls.Clear();
            _streamingTextBox = null;

            lblStatus.Text = "New conversation started.";
            txtQuestion.Clear();
            txtQuestion.Focus();
        }

        // ---------------------------------------------------------
        // لغو پاسخ
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
        // تغییر مدل
        // ---------------------------------------------------------
        private void cmbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedModel = cmbModel.SelectedItem as LLM;

            if (_selectedModel == null)
                return;

            lblStatus.Text = "Selected Model : " + _selectedModel.Provider + " > " + _selectedModel.Name;
            lblLLMRemarks.Text = (_selectedModel.UseProxy ? " (Proxied!)" : "") + _selectedModel.Remarks;
        }

        // ---------------------------------------------------------
        // کپی Status
        // ---------------------------------------------------------
        private void lblStatus_DoubleClick(object sender, EventArgs e)
        {
            if (sender is Label label && !string.IsNullOrEmpty(label.Text))
            {
                Clipboard.SetText(label.Text);
                MessageBox.Show("copied to clipboard");
            }
        }



        private void chkDebug_CheckedChanged(object sender, EventArgs e)
        {
            pnlDebug.Visible = chkDebug.Checked;
            // اگر روشن شد و هنوز چیزی لاگ نشده بود، یک پیام خوش‌آمد
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
            if (chkDebug.Checked)
                LogDebug($"[debug] enabled at {DateTime.Now:HH:mm:ss}");
        }

        /// <summary>
        /// اگر chkDebug خاموش باشد، هیچ کاری نمی‌کند (هزینه صفر).
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


        // ---------------------------------------------------------
        // ناوبری History
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
                {
                    Process.Start("notepad.exe", savedPath);
                }
            }
            else
            {
                MessageBox.Show("ذخیره لاگ انجام نشد.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}