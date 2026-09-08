using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleAIChat
{
    public partial class ChatPage01 : Form
    {
        private HttpClient _httpClient = new HttpClient();
        private int CurrentHeight = 0;
        private List<ChatMessage> _chatHistory = new List<ChatMessage>();
        private CancellationTokenSource? _cancellationTokenSource;

        public ChatPage01()
        {
            InitializeComponent();
            InitAll();
        }

        private void InitAll()
        {
            cmbModel.Items.Clear();
            cmbModel.Items.Add("gemma3-4b:latest");
            cmbModel.Items.Add("gemma2:2b");
            cmbModel.Items.Add("gemma3:1b");
            cmbModel.Items.Add("qwen2.5-coder:3b");
            cmbModel.SelectedIndex = 0;
            lblStatus.Text = "Ready. Ask a question.";
            _chatHistory.Clear();
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            string question = txtQuestion.Text.Trim();
            if (string.IsNullOrWhiteSpace(question))
            {
                MessageBox.Show("لطفاً یک سؤال وارد کنید.");
                return;
            }

            btnSend.Visible = false;
            btnCancell.Visible = true;
            _cancellationTokenSource = new CancellationTokenSource();

            _chatHistory.Add(new ChatMessage { Role = "user", Content = question });

            AddMessageBox("شما: " + question, false);
            txtQuestion.Clear();
            txtQuestion.Focus();
            lblStatus.Text = "Answering...";

            try
            {
                string answer;
                if (chkStream.Checked)
                    answer = await AskOllamaStream();
                else
                    answer = await AskOllamaFull();

                _chatHistory.Add(new ChatMessage { Role = "assistant", Content = answer });
                lblStatus.Text = "Answer is ready.";
            }
            catch (OperationCanceledException)
            {
                lblStatus.Text = "Response cancelled.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
                MessageBox.Show("خطا در ارتباط با مدل: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
                btnSend.Visible = true;
                btnCancell.Visible = false;
                txtQuestion.Focus();
            }
        }

        private TextBox AddMessageBox(string message, bool isAnswer)
        {
            // ساخت Panel به عنوان ظرف
            Panel container = new Panel();
            container.Width = pnlConversation.ClientSize.Width - 25;
            container.Left = 0;

            // لیبل
            Label lblTitle = new Label();
            lblTitle.Text = isAnswer ? cmbModel.SelectedItem?.ToString() : "User";
            lblTitle.Font = new Font("Tahoma", 8, FontStyle.Bold);
            lblTitle.ForeColor = isAnswer ? Color.DarkOrange : Color.DarkBlue;
            lblTitle.Width = container.Width - 10;
            lblTitle.Left = isAnswer ? 15 : container.Width-50;
            lblTitle.Top = 0;
            lblTitle.Height = 20;

            // تکست باکس
            TextBox txtMessage = new TextBox();
            txtMessage.Text = message;
            txtMessage.Multiline = true;
            txtMessage.ReadOnly = true;
            txtMessage.RightToLeft = RightToLeft;
            txtMessage.TextAlign = HorizontalAlignment.Right;
            txtMessage.Width = container.Width - 50;
            txtMessage.Left = isAnswer ? 15 : 50;
            txtMessage.Top = 20; 
            txtMessage.ScrollBars = ScrollBars.None;
            txtMessage.BackColor = isAnswer ? Color.LightYellow : Color.LightBlue;

            // **محاسبه ارتفاع و تنظیمش**
            txtMessage.Height = CalculateMessageHeight(txtMessage);

            // **تنظیم ارتفاع container بر اساس ارتفاع محاسبه شده**
            container.Height = txtMessage.Top + txtMessage.Height + 5;
            container.Tag = txtMessage; // برای دسترسی بعدی اگر نیاز شد

            // اضافه کردن به container
            container.Controls.Add(lblTitle);
            container.Controls.Add(txtMessage);

            // اضافه کردن container به پنل اصلی
            pnlConversation.Controls.Add(container);

            RecalculateLayout();
            pnlConversation.ScrollControlIntoView(container);
            return txtMessage;
        }

        private TextBox AddStreamingMessageBox()
        {
            TextBox txtMessage = new TextBox();
            txtMessage.Text = "پاسخ: ";
            txtMessage.Multiline = true;
            txtMessage.ReadOnly = true;
            txtMessage.Height = 120;
            txtMessage.Width = pnlConversation.ClientSize.Width - 60;
            txtMessage.Left = 0;
            txtMessage.ScrollBars = ScrollBars.Vertical;
            txtMessage.BackColor = Color.LightYellow;
            pnlConversation.Controls.Add(txtMessage);
            RecalculateLayout();
            pnlConversation.ScrollControlIntoView(txtMessage);
            return txtMessage;
        }

        private int CalculateMessageHeight(TextBox txtMessage)
        {
            int textWidth = txtMessage.ClientSize.Width - 8;
            if (textWidth < 1)
                textWidth = 1;

            using (Graphics graphics = txtMessage.CreateGraphics())
            {
                StringFormat format = new StringFormat();
                format.FormatFlags = StringFormatFlags.LineLimit;
                SizeF size = graphics.MeasureString(txtMessage.Text, txtMessage.Font, textWidth, format);
                int height = (int)Math.Ceiling(size.Height);
                height += 12;
                if (height < 40)
                    height = 40;
                return height;
            }
        }

        private void RecalculateLayout()
        {
            CurrentHeight = 0;
            foreach (Control control in pnlConversation.Controls)
            {
                control.Top = CurrentHeight;
                CurrentHeight += control.Height + 5;
            }
        }

        private async Task<string> AskOllamaStream()
        {
            string url = "http://localhost:11434/api/chat";
            var messages = new List<object>();

            foreach (var msg in _chatHistory)
                messages.Add(new { role = msg.Role, content = msg.Content });

            var requestData = new { model = cmbModel.SelectedItem?.ToString(), messages = messages, stream = true };

            string json = JsonSerializer.Serialize(requestData);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = content;

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, _cancellationTokenSource!.Token);
            response.EnsureSuccessStatusCode();

            Stream stream = await response.Content.ReadAsStreamAsync();
            StreamReader reader = new StreamReader(stream);
            StringBuilder fullAnswer = new StringBuilder();
            TextBox streamingTextBox = AddStreamingMessageBox();

            try
            {
                while (true)
                {
                    string? line = await reader.ReadLineAsync(_cancellationTokenSource!.Token);
                    if (line == null)
                        break;

                    try
                    {
                        using JsonDocument document = JsonDocument.Parse(line);
                        if (document.RootElement.TryGetProperty("message", out JsonElement messageElement))
                        {
                            string chunk = messageElement.GetProperty("content").GetString() ?? "";
                            chunk = chunk.Replace("\n", "\r\n");
                            fullAnswer.Append(chunk);
                            streamingTextBox.AppendText(chunk);
                            streamingTextBox.SelectionStart = streamingTextBox.Text.Length;
                            streamingTextBox.ScrollToCaret();
                        }

                        bool done = document.RootElement.GetProperty("done").GetBoolean();
                        if (done)
                            break;
                    }
                    catch (JsonException) { }
                }
            }
            finally
            {
                reader.Dispose();
                stream.Dispose();
                response.Dispose();
                request.Dispose();
                content.Dispose();
            }

            string answer = fullAnswer.ToString();
            pnlConversation.Controls.Remove(streamingTextBox);
            streamingTextBox.Dispose();
            RecalculateLayout();

            TextBox finalTextBox = AddMessageBox("مدل: " + answer, true);
            pnlConversation.ScrollControlIntoView(finalTextBox);
            return answer;
        }

        private async Task<string> AskOllamaFull()
        {
            string url = "http://localhost:11434/api/chat";
            var messages = new List<object>();

            foreach (var msg in _chatHistory)
                messages.Add(new { role = msg.Role, content = msg.Content });

            var requestData = new { model = cmbModel.SelectedItem?.ToString(), messages = messages, stream = false };

            string json = JsonSerializer.Serialize(requestData);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();
            response.Dispose();
            content.Dispose();

            using JsonDocument document = JsonDocument.Parse(responseJson);
            string answer = document.RootElement.GetProperty("message").GetProperty("content").GetString()?.Replace("\n", "\r\n") ?? "";

            TextBox finalTextBox = AddMessageBox("مدل: " + answer, true);
            pnlConversation.ScrollControlIntoView(finalTextBox);
            return answer;
        }

        private void btnNewChat_Click(object sender, EventArgs e)
        {
            _chatHistory.Clear();
            pnlConversation.Controls.Clear();
            CurrentHeight = 0;
            lblStatus.Text = "New conversation started.";
            txtQuestion.Clear();
            txtQuestion.Focus();
        }

        private class ChatMessage
        {
            public string Role { get; set; } = "";
            public string Content { get; set; } = "";
        }

        private void btnCancell_Click(object sender, EventArgs e)
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                lblStatus.Text = "Cancelling...";
            }
        }
    }
}