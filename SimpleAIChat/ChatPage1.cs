using ApcoAgentCore.Models;
using ApcoAgentCore.Services;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SimpleAIChat
{
    // ChatPage0 + دیباگ اختیاری + کنسل واقعی
    // از همان کنترل‌های موجود در Designer استفاده می‌کند.
    public partial class ChatPage1 : Form
    {
        //private readonly LLMService _llmService;
        private List<LLM> _models;
        private LLM _currentModel;
        private LLMService00 _llmService;

        // برای کنسل کردن درخواست جاری
        private CancellationTokenSource _cts;
        private bool _isBusy;

        // لاگر داخلی
        private readonly StringBuilder _debugLog = new StringBuilder();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer timerHide;


        public ChatPage1()
        {
            InitializeComponent();
            lblStatus.Text = "Initializing...";
            _llmService = new LLMService00();
            toolTip1 = new System.Windows.Forms.ToolTip();

            timerHide = new System.Windows.Forms.Timer();
            timerHide.Interval = 3000;
            timerHide.Tick += timerHide_Tick;

            LoadModels();
            InitializeComboBox();
            InitializeDebug();

            // دکمه کنسل در حالت عادی مخفی است (مطابق Designer)
            btnCancel.Visible = false;
        }

        // ---------- راه‌اندازی ----------

        private void LoadModels()
        {
            try
            {
                _models = LLMService00.LoadModels(false);

                if (_models.Count == 0)
                {
                    MessageBox.Show("No active models found!", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading models:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComboBox()
        {
            cmbModel.Items.Clear();

            foreach (var model in _models)
            {
                string displayName = $"{model.Provider} - {model.Name}";
                if (!string.IsNullOrEmpty(model.Remarks))
                    displayName += $" ({model.Remarks})";

                cmbModel.Items.Add(displayName);
            }

            if (cmbModel.Items.Count > 0)
                cmbModel.SelectedIndex = 0;

            lblStatus.Text = "Ready.";
        }

        private void InitializeDebug()
        {
            // پنل دیباگ = همان RichTextBox که در Designer هست
            txtDebug.ReadOnly = true;
            txtDebug.Visible = false;             // پیش‌فرض مخفی
            txtDebug.Font = new System.Drawing.Font("Consolas", 9F);
            txtDebug.ScrollBars = RichTextBoxScrollBars.Vertical;
        }

        private void cmbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbModel.SelectedIndex >= 0 && cmbModel.SelectedIndex < _models.Count)
            {
                _currentModel = _models[cmbModel.SelectedIndex];

                string status = $"Model: {_currentModel.Provider} - {_currentModel.Name}";
                status += _currentModel.Type == "Local" ? " (Local)" : " (Cloud)";

                if (!string.IsNullOrEmpty(_currentModel.Remarks))
                    status += $" | {_currentModel.Remarks}";

                lblStatus.Text = status;
                LogDebug($"Model selected → {status}");
            }
        }

        // ---------- ارسال پیام ----------

        private async void btnSend_Click(object sender, EventArgs e)
        {
            if (_isBusy) return;

            if (string.IsNullOrWhiteSpace(txtQuestion.Text))
            {
                MessageBox.Show("Please enter a question.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_currentModel == null)
            {
                MessageBox.Show("Please select a model.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _isBusy = true;
            SetBusyState(true);

            txtAnswer.Text = "";
            lblStatus.Text = "Sending request...";

            _cts = new CancellationTokenSource();
            _stopwatch.Restart();

            LogDebug("═══════════════════════════════════════");
            LogDebug($"▶ Request started at {DateTime.Now:HH:mm:ss.fff}");
            LogDebug($"  Model : {_currentModel.Provider} - {_currentModel.Name}");
            LogDebug($"  Stream: {chkStreamAnswer.Checked}");
            LogDebug($"  Prompt: {Truncate(txtQuestion.Text, 200)}");
            LogDebug("───────────────────────────────────────");

            try
            {
                bool useStream = chkStreamAnswer.Checked;

                string result = await _llmService.SendMessageAsync(
                    _currentModel,
                    txtQuestion.Text,
                    useStream,
                    onChunkReceived: (chunk) =>
                    {
                        // اگر کنسل شده باشد، دیگر UI را آپدیت نکن
                        if (_cts.IsCancellationRequested) return;

                        txtAnswer.Invoke(new Action(() =>
                        {
                            txtAnswer.Text = chunk.Replace("\n", "\r\n");
                            txtAnswer.SelectionStart = txtAnswer.Text.Length;
                            txtAnswer.ScrollToCaret();
                            lblStatus.Text = $"Receiving... ({chunk.Length} chars)";
                        }));
                    },
                    onDebugLog: chkDebug.Checked ? LogDebug : null,
                    cancellationToken: _cts.Token
                );

                _stopwatch.Stop();

                if (!useStream)
                {
                    txtAnswer.Text = result.Replace("\n", "\r\n");
                    lblStatus.Text = "✅ Answer is ready.";
                }
                else
                {
                    lblStatus.Text = $"✅ Stream completed. ({result.Length} chars)";
                }

                LogDebug("───────────────────────────────────────");
                LogDebug($"✔ Completed in {_stopwatch.ElapsedMilliseconds} ms");
                LogDebug($"  Answer length : {result.Length} chars");
                LogDebug($"  Answer preview: {Truncate(result, 300)}");
                LogDebug("═══════════════════════════════════════\r\n");
            }
            catch (OperationCanceledException)
            {
                _stopwatch.Stop();

                lblStatus.Text = "⏹ Cancelled by user.";
                LogDebug("───────────────────────────────────────");
                LogDebug($"⏹ Cancelled after {_stopwatch.ElapsedMilliseconds} ms");
                LogDebug("═══════════════════════════════════════\r\n");
            }
            catch (Exception ex)
            {
                _stopwatch.Stop();

                txtAnswer.Text = $"❌ Error:\r\n{ex.Message}";
                lblStatus.Text = $"❌ Error: {ex.Message}";

                LogDebug("───────────────────────────────────────");
                LogDebug($"✖ FAILED after {_stopwatch.ElapsedMilliseconds} ms");
                LogDebug($"  {ex.GetType().Name}: {ex.Message}");
                LogDebug($"  {ex.StackTrace}");
                LogDebug("═══════════════════════════════════════\r\n");
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
                _isBusy = false;
                SetBusyState(false);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                LogDebug("⏹ Cancel requested by user.");
                lblStatus.Text = "Cancelling...";
                _cts.Cancel();
            }
        }

        private void SetBusyState(bool busy)
        {
            btnSend.Enabled = !busy;
            btnCancel.Visible = busy;
            btnInfo.Enabled = !busy;
            cmbModel.Enabled = !busy;
            chkStreamAnswer.Enabled = !busy;
            txtQuestion.ReadOnly = busy;
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            if (_currentModel != null)
            {
                string info = _llmService.GetModelInfo(_currentModel);
                MessageBox.Show(info, "Model Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ---------- دیباگ ----------

        private void chkDebug_CheckedChanged(object sender, EventArgs e)
        {
            txtDebug.Visible = chkDebug.Checked;

            // اگر روشن شد و هنوز چیزی لاگ نشده بود، یک پیام خوش‌آمد
            if (chkDebug.Checked)
                LogDebug($"[debug] enabled at {DateTime.Now:HH:mm:ss}");
        }

        private void btnClearDebug_Click(object sender, EventArgs e)
        {
            _debugLog.Clear();
            txtDebug.Clear();
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

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Length <= max ? s : s.Substring(0, max) + "…";
        }

        // ---------- سابقه ----------

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

     /*   private void btnCopyLog_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(txtDebug.Text);
                MessageBox.Show("در کلیپبرد کپی شد", "موفق", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show(" کپی کلیپبرد انجام نشد.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/

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