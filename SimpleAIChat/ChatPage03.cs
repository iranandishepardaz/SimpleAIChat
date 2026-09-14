using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ApcoAgentCore.Models;
using ApcoAgentCore.Services;

namespace SimpleAIChat
{
    public partial class ChatPage03 : Form
    {
        private HttpClient _httpClient = new HttpClient();
        private readonly LLMService00 _llmService = new LLMService00();

        private readonly List<ChatMessage> _chatHistory = new List<ChatMessage>();
        private List<LLM> _models = new List<LLM>();

        private LLM _selectedModel;
        private CancellationTokenSource _cancellationTokenSource;

        public ChatPage03()
        {
            InitializeComponent();
            LoadModels();
        }

            private void LoadModels()
        {
            try
            {
                _models = LLMService00.LoadModels();
                _models = _models.FindAll(x => x.Enabled);

                cmbModel.DataSource = _models;
                cmbModel.DisplayMember = "Id";

                if (_models.Count > 0)
                    cmbModel.SelectedIndex = 0;

                lblStatus.Text = "Models loaded.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading models.";

                MessageBox.Show("خطا در خواندن فایل مدل‌ها:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (string.IsNullOrWhiteSpace(question))
            {
                MessageBox.Show("لطفاً یک سؤال وارد کنید.");
                return;
            }

            StartAnswering();

            _chatHistory.Add(new ChatMessage { Role = "user", Content = question });

            AddMessageBox(question, false);

            txtQuestion.Clear();
            txtQuestion.Focus();

            try
            {
                string answer;

                if (chkStream.Checked)
                    answer = await AskStream();
                else
                    answer = await AskFull();

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
                StopAnswering();
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
        // ساخت لیست پیام‌ها برای API
        // ---------------------------------------------------------
        private List<object> BuildMessages()
        {
            List<object> messages = new List<object>();

            foreach (ChatMessage message in _chatHistory)
            {
                messages.Add(new
                {
                    role = message.Role,
                    content = message.Content
                });
            }

            return messages;
        }

        // ---------------------------------------------------------
        // ساخت درخواست HTTP
        // ---------------------------------------------------------
        private HttpRequestMessage CreateRequest(string json)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _selectedModel.Endpoint);

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            string apiKey = _selectedModel.GetApiKey();

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiKey);
            }

            return request;
        }

        // ---------------------------------------------------------
        // ارسال Stream
        // ---------------------------------------------------------
        private async Task<string> AskStream()
        {
            bool isOllama = _selectedModel.Provider == "Ollama";

            object requestData;

            if (isOllama)
            {
                requestData = new
                {
                    model = _selectedModel.Name,
                    messages = BuildMessages(),
                    stream = true,
                    options = new
                    {
                        temperature = _selectedModel.Temperature,
                        num_predict = _selectedModel.MaxTokens
                    }
                };
            }
            else
            {
                requestData = new
                {
                    model = _selectedModel.Name,
                    messages = BuildMessages(),
                    stream = true,
                    temperature = _selectedModel.Temperature,
                    max_tokens = _selectedModel.MaxTokens
                };
            }

            string json = JsonSerializer.Serialize(requestData);

            using (HttpRequestMessage request = CreateRequest(json))
            using (HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, _cancellationTokenSource.Token))
            {
                response.EnsureSuccessStatusCode();

                using (Stream stream = await response.Content.ReadAsStreamAsync())
                using (StreamReader reader = new StreamReader(stream))
                {
                    TextBox textBox = AddStreamingMessageBox();
                    StringBuilder answer = new StringBuilder();

                    try
                    {
                        while (true)
                        {
                            string line = await reader.ReadLineAsync(_cancellationTokenSource.Token);

                            if (line == null)
                                break;

                            if (string.IsNullOrWhiteSpace(line))
                                continue;

                            string chunk;
                            bool done;

                            if (isOllama)
                            {
                                chunk = ParseOllamaStreamLine(line, out done);

                                if (!string.IsNullOrEmpty(chunk))
                                    AppendChunk(answer, textBox, chunk);

                                if (done)
                                    break;
                            }
                            else
                            {
                                chunk = ParseOpenAIStreamLine(line, out done);

                                if (!string.IsNullOrEmpty(chunk))
                                    AppendChunk(answer, textBox, chunk);

                                if (done)
                                    break;
                            }
                        }

                        return answer.ToString();
                    }
                    finally
                    {
                        pnlConversation.Controls.Remove(textBox);
                        textBox.Dispose();

                        RecalculateLayout();

                        TextBox finalTextBox = AddMessageBox(answer.ToString(), true);

                        pnlConversation.ScrollControlIntoView(finalTextBox);
                    }
                }
            }
        }

        // ---------------------------------------------------------
        // پردازش یک خط Stream مربوط به Ollama
        // ---------------------------------------------------------
        private string ParseOllamaStreamLine(string line, out bool done)
        {
            done = false;

            try
            {
                using (JsonDocument document = JsonDocument.Parse(line))
                {
                    JsonElement root = document.RootElement;

                    if (root.TryGetProperty("message", out JsonElement message))
                    {
                        if (message.TryGetProperty("content", out JsonElement content))
                        {
                            string text = content.GetString() ?? "";

                            if (root.TryGetProperty("done", out JsonElement doneElement))
                                done = doneElement.GetBoolean();

                            return text;
                        }
                    }

                    if (root.TryGetProperty("done", out JsonElement doneOnly))
                        done = doneOnly.GetBoolean();
                }
            }
            catch (JsonException)
            {
                // خط نامعتبر را نادیده می‌گیریم.
            }

            return "";
        }


        // ---------------------------------------------------------
        // پردازش یک خط Stream مربوط به OpenAI / Cloudflare
        // ---------------------------------------------------------
        private string ParseOpenAIStreamLine(string line, out bool done)
        {
            done = false;

            if (!line.StartsWith("data:"))
            {
                CheckStreamError(line);
                return "";
            }

            string data = line.Substring(5).Trim();

            if (data == "[DONE]")
            {
                done = true;
                return "";
            }

            try
            {
                using (JsonDocument document = JsonDocument.Parse(data))
                {
                    JsonElement root = document.RootElement;

                    // OpenAI / APIهای سازگار
                    if (root.TryGetProperty("choices", out JsonElement choices))
                    {
                        if (choices.GetArrayLength() == 0)
                            return "";

                        JsonElement choice = choices[0];

                        // حالت delta
                        if (choice.TryGetProperty("delta", out JsonElement delta))
                            return GetText(delta);

                        // بعضی APIها از message استفاده می‌کنند.
                        if (choice.TryGetProperty("message", out JsonElement message))
                            return GetText(message);

                        return "";
                    }

                    // Ollama یا APIهایی که message دارند
                    if (root.TryGetProperty("message", out JsonElement ollamaMessage))
                        return GetText(ollamaMessage);

                    // Cloudflare Workers AI
                    if (root.TryGetProperty("response", out JsonElement response))
                        return GetText(response);
                }
            }
            catch (JsonException)
            {
                // JSON ناقص یا نامعتبر را نادیده می‌گیریم.
            }

            return "";
        }


        // ---------------------------------------------------------
        // استخراج متن از String یا Object
        // ---------------------------------------------------------
        private string GetText(JsonElement element)
        {
            // حالت معمول:
            // "content": "Hello"
            // "response": "Hello"
            if (element.ValueKind == JsonValueKind.String)
                return element.GetString() ?? "";

            // اگر Object بود، دنبال فیلدهای متنی معمول می‌گردیم.
            if (element.ValueKind == JsonValueKind.Object)
            {
                string text = GetStringProperty(element, "content");

                if (!string.IsNullOrEmpty(text))
                    return text;

                text = GetStringProperty(element, "text");

                if (!string.IsNullOrEmpty(text))
                    return text;

                text = GetStringProperty(element, "response");

                if (!string.IsNullOrEmpty(text))
                    return text;
            }

            return "";
        }


        // ---------------------------------------------------------
        // بررسی خطای API
        // ---------------------------------------------------------
        private void CheckStreamError(string line)
        {
            try
            {
                using (JsonDocument document = JsonDocument.Parse(line))
                {
                    JsonElement root = document.RootElement;

                    if (root.TryGetProperty("error", out JsonElement error))
                    {
                        string message = GetStringProperty(error, "message");

                        if (string.IsNullOrWhiteSpace(message))
                            message = "Unknown API error";

                        throw new Exception("API Error: " + message);
                    }
                }
            }
            catch (JsonException)
            {
                // اگر JSON نبود، نادیده گرفته می‌شود.
            }
        }

        // ---------------------------------------------------------
        // گرفتن مقدار یک property رشته‌ای
        // ---------------------------------------------------------
        private string GetStringProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement value))
            {
                return "";
            }

            return value.ValueKind == JsonValueKind.String ? value.GetString() ?? "" : "";
        }

        // ---------------------------------------------------------
        // اضافه کردن یک Chunk به پاسخ در حال دریافت
        // ---------------------------------------------------------
        private void AppendChunk(StringBuilder answer, TextBox textBox, string chunk)
        {
            answer.Append(chunk);

            string displayText = chunk.Replace("\n", "\r\n");

            textBox.AppendText(displayText);
            textBox.SelectionStart = textBox.Text.Length;
            textBox.ScrollToCaret();
        }

        // ---------------------------------------------------------
        // ارسال پاسخ کامل
        // ---------------------------------------------------------
        private async Task<string> AskFull()
        {
            bool isOllama = _selectedModel.Provider == "Ollama";

            object requestData;

            if (isOllama)
            {
                requestData = new
                {
                    model = _selectedModel.Name,
                    messages = BuildMessages(),
                    stream = false,
                    options = new
                    {
                        temperature = _selectedModel.Temperature,
                        num_predict = _selectedModel.MaxTokens
                    }
                };
            }
            else
            {
                requestData = new
                {
                    model = _selectedModel.Name,
                    messages = BuildMessages(),
                    stream = false,
                    temperature = _selectedModel.Temperature,
                    max_tokens = _selectedModel.MaxTokens
                };
            }

            string json = JsonSerializer.Serialize(requestData);

            using (HttpRequestMessage request = CreateRequest(json))
            using (HttpResponseMessage response = await _httpClient.SendAsync(request, _cancellationTokenSource.Token))
            {
                response.EnsureSuccessStatusCode();

                string responseJson =
                    await response.Content.ReadAsStringAsync(_cancellationTokenSource.Token);

                string answer = ParseFullResponse(responseJson);

                TextBox textBox = AddMessageBox(answer, true);

                pnlConversation.ScrollControlIntoView(textBox);

                return answer;
            }
        }

        // ---------------------------------------------------------
        // استخراج متن از پاسخ کامل API
        // ---------------------------------------------------------
        private string ParseFullResponse(string responseJson)
        {
            try
            {
                using (JsonDocument document = JsonDocument.Parse(responseJson))
                {
                    JsonElement root = document.RootElement;

                    // OpenAI
                    if (root.TryGetProperty("choices", out JsonElement choices))
                    {
                        if (choices.GetArrayLength() > 0)
                        {
                            JsonElement choice = choices[0];

                            if (choice.TryGetProperty("message", out JsonElement message))
                            {
                                string content = GetStringProperty(message, "content");
                                if (!string.IsNullOrEmpty(content))
                                    return content;
                            }
                        }
                    }

                    // Ollama
                    if (root.TryGetProperty("message", out JsonElement ollamaMessage))
                    {
                        string content = GetStringProperty(ollamaMessage, "content");
                        if (!string.IsNullOrEmpty(content))
                            return content;
                    }

                    // Cloudflare Workers AI
                    if (root.TryGetProperty("response", out JsonElement response))
                    {
                        return response.GetString() ?? "";
                    }

                    // برخی APIهای ساده
                    if (root.TryGetProperty("result", out JsonElement result))
                    {
                        return result.GetString() ?? "";
                    }
                }
            }
            catch (JsonException)
            {
                // در صورت JSON نامعتبر، خود پاسخ را برمی‌گردانیم.
            }

            // برای دیباگ، JSON خام نمایش داده می‌شود.
            return responseJson;
        }

        // ---------------------------------------------------------
        // ساخت پیام عادی
        // ---------------------------------------------------------
        private TextBox AddMessageBox(string message, bool isAnswer)
        {
            int width = pnlConversation.ClientSize.Width - 25;

            Panel container = new Panel
            {
                Width = width,
                Left = 0,
                Height = 0
            };

            Label title = new Label
            {
                Text = isAnswer ? (_selectedModel?.Name ?? "Assistant") : "User",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = isAnswer ? Color.DarkOrange : Color.DarkBlue,
                Width = width - 20,
                Left = isAnswer ? 15 : width - 50,
                Top = 0,
                Height = 25,
                TextAlign = isAnswer ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight
            };

            TextBox textBox = new TextBox
            {
                Text = string.IsNullOrWhiteSpace(message) ? (isAnswer ? "پاسخی دریافت نشد." : "(پیام خالی)") : message.Replace("\n", "\r\n"),
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
                Padding = new Padding(5)
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

                Font = new Font(
                    "Tahoma",
                    10,
                    FontStyle.Regular),

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
                format.FormatFlags =
                    StringFormatFlags.LineLimit;

                SizeF size = graphics.MeasureString(
                    textBox.Text,
                    textBox.Font,
                    width,
                    format);

                int height =
                    (int)Math.Ceiling(size.Height) + 12;

                return Math.Max(height, 40);
            }
        }

        // ---------------------------------------------------------
        // چیدمان پیام‌ها
        // ---------------------------------------------------------
        private void RecalculateLayout()
        {
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
            _httpClient?.Dispose();
            _httpClient = ProxyFactory.Create(_selectedModel);

            if (_selectedModel == null)
                return;

            lblStatus.Text = "Selected Model : " + _selectedModel.Provider + " > " + _selectedModel.Name;
            lblLLMRemarks.Text = (_selectedModel.UseProxy ? " (Proxied!)" : "") + _selectedModel.Remarks;
        }

        // ---------------------------------------------------------
        // کپی Status و نمایش Proxy
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
        // مدل پیام
        // ---------------------------------------------------------
        private class ChatMessage
        {
            public string Role { get; set; } = "";
            public string Content { get; set; } = "";
        }

        private class LLMResponse
        {
            public string Content { get; set; } = "";

            public int PromptTokens { get; set; }

            public int CompletionTokens { get; set; }

            public int TotalTokens { get; set; }

            public TimeSpan ResponseTime { get; set; }

            public bool IsStream { get; set; }
        }
    }
}
