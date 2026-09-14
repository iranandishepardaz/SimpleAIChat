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
using ApcoAgentCore.Models;
using ApcoAgentCore.Services;


namespace SimpleAIChat
{
    public partial class ChatPage02 : Form
    {
        private HttpClient _httpClient = new HttpClient();
        private int CurrentHeight = 0;
        private List<ChatMessage> _chatHistory = new List<ChatMessage>();
        private CancellationTokenSource? _cancellationTokenSource;

        private List<LLM> _models = new List<LLM>();
        private LLM _selectedModel;

        public ChatPage02()
        {
            InitializeComponent();
            InitAll();
        }

        private void InitAll()
        {
            // ---------------------------------------------------------
            // مدل‌ها را از فایل JSON می‌خوانیم.
            // ---------------------------------------------------------
            try
            {
                _models = LLMService00.LoadModels(false);


                // -----------------------------------------------------
                // فقط مدل‌هایی که Enabled هستند را در ComboBox
                // نمایش می‌دهیم.
                // -----------------------------------------------------
                _models = _models.FindAll(x => x.Enabled);


                // -----------------------------------------------------
                // لیست مدل‌ها را مستقیماً به ComboBox متصل می‌کنیم.
                //
                // DisplayMember مشخص می‌کند چه چیزی به کاربر نمایش داده شود.
                // -----------------------------------------------------
                cmbModel.DataSource = _models;
                cmbModel.DisplayMember = "Name";


                // اولین مدل را انتخاب می‌کنیم.
                if (_models.Count > 0)
                    cmbModel.SelectedIndex = 0;


                lblStatus.Text = "Models loaded.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading models.";

                MessageBox.Show(
                    "خطا در خواندن فایل مدل‌ها:\r\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }


            // تاریخچه گفتگوی جدید را خالی می‌کنیم.
            _chatHistory.Clear();
        }

        private LLM? GetSelectedModel()
        {
            // ---------------------------------------------------------
            // ComboBox در حال حاضر مستقیماً به List<LLM> متصل است.
            //
            // بنابراین SelectedItem همان شیء LLM انتخاب‌شده است.
            // ---------------------------------------------------------
            return cmbModel.SelectedItem as LLM;

        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            // _selectedModel = GetSelectedModel();

            if (_selectedModel == null)
            {
                MessageBox.Show("لطفاً یک مدل انتخاب کنید.");
                return;
            }

            string question = txtQuestion.Text.Trim();
            if (string.IsNullOrWhiteSpace(question))
            {
                MessageBox.Show("لطفاً یک سؤال وارد کنید.");
                return;
            }

            lblStatus.Text = "Answering...";

            btnSend.Visible = false;
            btnCancell.Visible = true;
            _cancellationTokenSource = new CancellationTokenSource();

            _chatHistory.Add(new ChatMessage { Role = "user", Content = question });

            AddMessageBox(question, false);
            txtQuestion.Clear();
            txtQuestion.Focus();
            lblStatus.Text = "Answering...";

            try
            {
                string answer;
                if (chkStream.Checked)
                    if (_selectedModel.Provider == "Ollama")
                        answer = await AskOllamaStream();
                    else
                        answer = await AskOpenAIStream();
                else
                    if (_selectedModel.Provider == "Ollama")
                    answer = await AskOllamaFull();
                else
                    answer = await AskOpenAIFull();


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
            container.AutoSize = false;

            // لیبل عنوان
            Label lblTitle = new Label();
            lblTitle.Text = isAnswer ? _selectedModel?.Name ?? "Assistant" : "User";
            lblTitle.Font = new Font("Tahoma", 9, FontStyle.Bold);
            lblTitle.ForeColor = isAnswer ? Color.DarkOrange : Color.DarkBlue;
            lblTitle.Width = container.Width - 20;
            lblTitle.Left = isAnswer ? 15 : container.Width - 50;
            lblTitle.Top = 0;
            lblTitle.Height = 25;
            lblTitle.TextAlign = isAnswer ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight;

            // تکست باکس برای نمایش متن
            TextBox txtMessage = new TextBox();
            txtMessage.Text = message;
            txtMessage.Multiline = true;
            txtMessage.ReadOnly = true;
            txtMessage.RightToLeft = RightToLeft;
            txtMessage.TextAlign = HorizontalAlignment.Right;
            txtMessage.Width = container.Width - 50;
            txtMessage.Left = isAnswer ? 15 : 35;
            txtMessage.Top = 28;
            txtMessage.ScrollBars = ScrollBars.None;
            txtMessage.BorderStyle = BorderStyle.FixedSingle;
            txtMessage.BackColor = isAnswer ? Color.LightYellow : Color.LightBlue;
            txtMessage.Font = new Font("Tahoma", 10, FontStyle.Regular);
            txtMessage.Padding = new Padding(5);

            // محاسبه ارتفاع مناسب
            txtMessage.Height = CalculateMessageHeight(txtMessage);

            // اگر متن خالی بود، یک پیام پیش‌فرض نمایش بده
            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                txtMessage.Text = isAnswer ? "پاسخی دریافت نشد." : "(پیام خالی)";
                txtMessage.Height = CalculateMessageHeight(txtMessage);
            }

            // تنظیم ارتفاع container
            container.Height = txtMessage.Top + txtMessage.Height + 10;
            container.Tag = txtMessage;

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
            txtMessage.Text = "⏳ در حال دریافت پاسخ...\r\n\r\n";
            txtMessage.Multiline = true;
            txtMessage.ReadOnly = true;
            txtMessage.RightToLeft = RightToLeft;
            txtMessage.TextAlign = HorizontalAlignment.Right;
            txtMessage.Height = 150;
            txtMessage.Width = pnlConversation.ClientSize.Width - 50;
            txtMessage.Left = 15;
            txtMessage.ScrollBars = ScrollBars.Vertical;
            txtMessage.BackColor = Color.LightYellow;
            txtMessage.BorderStyle = BorderStyle.FixedSingle;
            txtMessage.Font = new Font("Tahoma", 10, FontStyle.Regular);
            txtMessage.Padding = new Padding(5);

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

        private async Task<string> AskOpenAIStream()
        {
            var messages = new List<object>();

            foreach (var msg in _chatHistory)
                messages.Add(new { role = msg.Role, content = msg.Content });

            var requestData = new
            {
                model = _selectedModel.Name,
                messages = messages,
                stream = true,
                temperature = _selectedModel.Temperature,
                max_tokens = _selectedModel.MaxTokens
            };

            string json = JsonSerializer.Serialize(requestData);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _selectedModel.Endpoint);
            request.Content = content;

            string apiKey = _selectedModel.GetApiKey();
            if (!string.IsNullOrEmpty(apiKey))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            }

            HttpResponseMessage response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                _cancellationTokenSource!.Token);

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

                    // خطوط خالی را نادیده بگیر
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // اگر خط با "data:" شروع نشد، ممکن است خطای JSON باشد
                    if (!line.StartsWith("data:"))
                    {
                        // برخی از سرویس‌ها ممکن است خطا را به صورت JSON ساده برگردانند
                        try
                        {
                            using JsonDocument errorDoc = JsonDocument.Parse(line);
                            if (errorDoc.RootElement.TryGetProperty("error", out JsonElement error))
                            {
                                string errorMsg = error.TryGetProperty("message", out JsonElement msg)
                                    ? msg.GetString() ?? "Unknown error"
                                    : "Unknown error";
                                throw new Exception($"API Error: {errorMsg}");
                            }
                        }
                        catch (JsonException)
                        {
                            // اگر JSON نبود، نادیده بگیر
                        }
                        continue;
                    }

                    string data = line.Substring(5).Trim();

                    if (data == "[DONE]")
                        break;

                    try
                    {
                        using JsonDocument document = JsonDocument.Parse(data);
                        JsonElement root = document.RootElement;

                        // ✅ ابتدا فرمت استاندارد OpenAI را بررسی کن
                        if (root.TryGetProperty("choices", out JsonElement choices))
                        {
                            if (choices.GetArrayLength() > 0)
                            {
                                JsonElement choice = choices[0];

                                // حالت stream: delta
                                if (choice.TryGetProperty("delta", out JsonElement delta))
                                {
                                    if (delta.TryGetProperty("content", out JsonElement contentElement))
                                    {
                                        if (contentElement.ValueKind == JsonValueKind.String)
                                        {
                                            string chunk = contentElement.GetString() ?? "";
                                            if (!string.IsNullOrEmpty(chunk))
                                            {
                                                fullAnswer.Append(chunk);
                                                chunk = chunk.Replace("\n", "\r\n");
                                                streamingTextBox.AppendText(chunk);
                                                streamingTextBox.SelectionStart = streamingTextBox.Text.Length;
                                                streamingTextBox.ScrollToCaret();
                                            }
                                        }
                                    }
                                }
                                // حالت full (برخی سرویس‌ها در stream ممکن است message بدهند)
                                else if (choice.TryGetProperty("message", out JsonElement message))
                                {
                                    if (message.TryGetProperty("content", out JsonElement contentElement))
                                    {
                                        if (contentElement.ValueKind == JsonValueKind.String)
                                        {
                                            string chunk = contentElement.GetString() ?? "";
                                            if (!string.IsNullOrEmpty(chunk))
                                            {
                                                fullAnswer.Append(chunk);
                                                chunk = chunk.Replace("\n", "\r\n");
                                                streamingTextBox.AppendText(chunk);
                                                streamingTextBox.SelectionStart = streamingTextBox.Text.Length;
                                                streamingTextBox.ScrollToCaret();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        // ✅ اگر فرمت OpenAI نبود، فرمت Ollama را بررسی کن
                        else if (root.TryGetProperty("message", out JsonElement ollamaMessage))
                        {
                            if (ollamaMessage.TryGetProperty("content", out JsonElement contentElement))
                            {
                                string chunk = contentElement.GetString() ?? "";
                                if (!string.IsNullOrEmpty(chunk))
                                {
                                    fullAnswer.Append(chunk);
                                    chunk = chunk.Replace("\n", "\r\n");
                                    streamingTextBox.AppendText(chunk);
                                    streamingTextBox.SelectionStart = streamingTextBox.Text.Length;
                                    streamingTextBox.ScrollToCaret();
                                }
                            }
                        }
                        // ✅ فرمت Cloudflare Workers AI
                        else if (root.TryGetProperty("response", out JsonElement responseElement))
                        {
                            string chunk = responseElement.GetString() ?? "";
                            if (!string.IsNullOrEmpty(chunk))
                            {
                                fullAnswer.Append(chunk);
                                chunk = chunk.Replace("\n", "\r\n");
                                streamingTextBox.AppendText(chunk);
                                streamingTextBox.SelectionStart = streamingTextBox.Text.Length;
                                streamingTextBox.ScrollToCaret();
                            }
                        }
                    }
                    catch (JsonException)
                    {
                        // خط JSON نامعتبر را نادیده بگیر
                    }
                }

                return fullAnswer.ToString();
            }
            finally
            {
                reader.Dispose();
                stream.Dispose();
                response.Dispose();
                request.Dispose();
                content.Dispose();
                pnlConversation.Controls.Remove(streamingTextBox);
                streamingTextBox.Dispose();
                RecalculateLayout();

                TextBox finalTextBox = AddMessageBox(fullAnswer.ToString(), true);
                pnlConversation.ScrollControlIntoView(finalTextBox);
            }
        }

        private async Task<string> AskOpenAIFull()
        {
            var messages = new List<object>();

            foreach (var msg in _chatHistory)
                messages.Add(new { role = msg.Role, content = msg.Content });

            var requestData = new
            {
                model = _selectedModel.Name,
                messages = messages,
                stream = false,
                temperature = _selectedModel.Temperature,
                max_tokens = _selectedModel.MaxTokens
            };

            string json = JsonSerializer.Serialize(requestData);

            StringContent content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            HttpRequestMessage request = new HttpRequestMessage(
                HttpMethod.Post,
                _selectedModel.Endpoint);

            request.Content = content;

            string apiKey = _selectedModel.GetApiKey();
            if (!string.IsNullOrEmpty(apiKey))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer",
                        apiKey);
            }

            HttpResponseMessage response = await _httpClient.SendAsync(
                request,
                _cancellationTokenSource!.Token);

            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();

            response.Dispose();
            request.Dispose();
            content.Dispose();

            string answer = await ParseFullResponse(responseJson);

            answer = answer.Replace("\n", "\r\n");

            TextBox finalTextBox = AddMessageBox(answer, true);
            pnlConversation.ScrollControlIntoView(finalTextBox);

            return answer;
        }

        // ✅ متد کمکی برای parse پاسخ کامل با پشتیبانی از فرمت‌های مختلف
        private async Task<string> ParseFullResponse(string responseJson)
        {
            try
            {
                using JsonDocument document = JsonDocument.Parse(responseJson);
                JsonElement root = document.RootElement;

                // ✅ فرمت OpenAI
                if (root.TryGetProperty("choices", out JsonElement choices))
                {
                    if (choices.GetArrayLength() > 0)
                    {
                        JsonElement choice = choices[0];
                        if (choice.TryGetProperty("message", out JsonElement message))
                        {
                            if (message.TryGetProperty("content", out JsonElement content))
                            {
                                return content.GetString() ?? "";
                            }
                        }
                    }
                }

                // ✅ فرمت Ollama
                if (root.TryGetProperty("message", out JsonElement ollamaMessage))
                {
                    if (ollamaMessage.TryGetProperty("content", out JsonElement content))
                    {
                        return content.GetString() ?? "";
                    }
                }

                // ✅ فرمت Cloudflare Workers AI
                if (root.TryGetProperty("response", out JsonElement response))
                {
                    return response.GetString() ?? "";
                }

                // ✅ فرمت‌های دیگر (مثلاً برخی از APIهای ساده)
                if (root.TryGetProperty("result", out JsonElement result))
                {
                    return result.GetString() ?? "";
                }

                // اگر هیچکدام نبود، کل JSON را به عنوان پاسخ برگردان (برای دیباگ)
                return responseJson;
            }
            catch
            {
                return responseJson;
            }
        }
        
        private async Task<string> AskOllamaStream()
        {
            var messages = new List<object>();

            foreach (var msg in _chatHistory)
                messages.Add(new { role = msg.Role, content = msg.Content });

            var requestData = new
            {
                model = _selectedModel.Name,
                messages = messages,
                stream = true,
                options = new
                {
                    temperature = _selectedModel.Temperature,
                    num_predict = _selectedModel.MaxTokens
                }
            };

            string json = JsonSerializer.Serialize(requestData);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _selectedModel.Endpoint);

            request.Content = content;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_selectedModel.GetApiKey()}");
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

                return fullAnswer.ToString();
            }
            finally
            {
                reader.Dispose();
                stream.Dispose();
                response.Dispose();
                request.Dispose();
                content.Dispose();
                pnlConversation.Controls.Remove(streamingTextBox);
                streamingTextBox.Dispose();
                RecalculateLayout();

                TextBox finalTextBox = AddMessageBox(fullAnswer.ToString(), true);
                pnlConversation.ScrollControlIntoView(finalTextBox);
            }
        }

        private async Task<string> AskOllamaFull()
        {
            var messages = new List<object>();

            foreach (var msg in _chatHistory)
                messages.Add(new { role = msg.Role, content = msg.Content });
            // ---------------------------------------------------------
            // اطلاعات درخواست
            // ---------------------------------------------------------
            var requestData = new
            {
                model = _selectedModel.Name,
                messages = messages,
                stream = false,
                options = new
                {
                    temperature = _selectedModel.Temperature,
                    num_predict = _selectedModel.MaxTokens
                }
            };

            string json = JsonSerializer.Serialize(requestData);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(_selectedModel.Endpoint, content);

            // اضافه کردن API Key به هدر
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_selectedModel.GetApiKey()}");


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

        private void cmbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedModel = GetSelectedModel();
            lblStatus.Text = "Selected Model :" + _selectedModel.Provider + " > " + _selectedModel.Name;
        }

        private void lblStatus_DoubleClick(object sender, EventArgs e)
        {
            if (sender is Label label && !string.IsNullOrEmpty(label.Text))
            {
                Clipboard.SetText(label.Text);
                MessageBox.Show("copied to clipboard");
                MessageBox.Show(
    "HTTP_PROXY = " + Environment.GetEnvironmentVariable("HTTP_PROXY") + "\r\n" +
    "HTTPS_PROXY = " + Environment.GetEnvironmentVariable("HTTPS_PROXY"));
            }
        }

    }
}