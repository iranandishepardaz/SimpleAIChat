using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using ApcoAgentCore.Models;

namespace ApcoAgentCore.Services
{
    public class LLMService
    {
        public LLMService() { }

        // ---------------------------------------------------------
        // بارگذاری مدل‌ها
        // ---------------------------------------------------------
        public static List<LLM> LoadModels(bool LoadAll = true)
        {
            string filePath = PathService.ModelsFilePath;

            if (!File.Exists(filePath))
            {
                PathService.EnsureAppDataFolderExists();
                File.WriteAllText(filePath, LLM.DefaultModelsJson, Encoding.UTF8);
            }

            string json = File.ReadAllText(filePath);

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;

            List<LLM>? models = JsonSerializer.Deserialize<List<LLM>>(json, options);
            if (!LoadAll && models != null)
                models = models.FindAll(m => m.Enabled);

            return models ?? new List<LLM>();
        }

        public static void SaveModels(List<LLM> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string json = JsonSerializer.Serialize(models, options);
            File.WriteAllText(PathService.ModelsFilePath, json, Encoding.UTF8);
        }

        // ---------------------------------------------------------
        // ساخت ChatRequest از مدل + پریست + تاریخچه
        // ---------------------------------------------------------
        public static ChatRequest BuildRequest(LLM model, Preset preset, List<ChatMessage> history)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var messages = new List<ChatMessage>();

            if (preset != null && !string.IsNullOrWhiteSpace(preset.SystemPrompt))
            {
                messages.Add(new ChatMessage
                {
                    Role = "system",
                    Content = preset.SystemPrompt
                });
            }

            if (preset != null && !string.IsNullOrWhiteSpace(preset.KnowledgeFile))
            {
                string knowledge = TryReadFile(preset.KnowledgeFile);
                if (!string.IsNullOrWhiteSpace(knowledge))
                {
                    messages.Add(new ChatMessage
                    {
                        Role = "system",
                        Content = "Knowledge:\n" + knowledge
                    });
                }
            }

            if (history != null)
                messages.AddRange(history);

            return new ChatRequest
            {
                Model = model,
                Messages = messages,
                Temperature = preset?.Temperature ?? model.Temperature,
                MaxTokens = preset?.MaxTokens ?? model.MaxTokens,
                TopP = preset?.TopP ?? null,
                UseStream = false
            };
        }

        private static string TryReadFile(string path)
        {
            try
            {
                if (!File.Exists(path)) return "";
                return File.ReadAllText(path);
            }
            catch
            {
                return "";
            }
        }




        // ---------------------------------------------------------
        // متد اصلی ارسال پیام
        // ---------------------------------------------------------
        public async Task<string> SendMessageAsync(
            ChatRequest request,
            Action<string> onChunkReceived = null,
            Action<string> onDebugLog = null,
            Action<string> onVerboseLog = null,
            DebugLevel debugLevel = DebugLevel.Normal,
            CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Model == null) throw new ArgumentException("Request.Model is required.");
            if (request.Messages == null || request.Messages.Count == 0)
                throw new ArgumentException("Request.Messages cannot be empty.");

            using var client = await PrepareHttpClientAsync(request.Model);

            if (request.UseStream)
                return await SendStreamMessageAsync(client, request, onChunkReceived, onDebugLog, onVerboseLog, debugLevel, cancellationToken);
            else
                return await SendNormalMessageAsync(client, request, onDebugLog, onVerboseLog, debugLevel, cancellationToken);
        }


        /// <summary>
        /// ارسال درخواست با ابزارها (Function Calling).
        /// 
        /// اگر مدل ابزاری را صدا بزند، آن را اجرا می‌کند، نتیجه را به مدل
        /// برمی‌گرداند، و دوباره می‌پرسد. این حلقه تا وقتی مدل پاسخ نهایی
        /// بدهد (بدون tool_calls) یا به سقف مراحل برسد ادامه می‌یابد.
        /// 
        /// فعلاً فقط حالت non-stream پشتیبانی می‌شود.
        /// </summary>
        public async Task<string> SendWithToolsAsync(
            ChatRequest request,
            Action<string> onDebugLog = null,
            Action<string> onVerboseLog = null,
            DebugLevel debugLevel = DebugLevel.Normal,
            int maxSteps = 5,
            CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Model == null) throw new ArgumentException("Request.Model is required.");
            if (request.Messages == null || request.Messages.Count == 0)
                throw new ArgumentException("Request.Messages cannot be empty.");
            if (request.Tools == null || request.Tools.Count == 0)
                throw new ArgumentException("Request.Tools cannot be empty for SendWithToolsAsync.");

            using var client = await PrepareHttpClientAsync(request.Model);

            for (int step = 0; step < maxSteps; step++)
            {
                onDebugLog?.Invoke($"[tools] step {step + 1}/{maxSteps}");

                // ۱) ساخت درخواست با tools
                using var httpRequest = CreateToolRequest(request);

                if (debugLevel == DebugLevel.Verbose)
                    onVerboseLog?.Invoke($"[HTTP →] Body: {await SafeReadContentAsync(httpRequest.Content)}");

                // ۲) ارسال
                HttpResponseMessage response = await client.SendAsync(httpRequest, cancellationToken);
                response.EnsureSuccessStatusCode();

                string responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

                if (debugLevel == DebugLevel.Verbose)
                    onVerboseLog?.Invoke($"[HTTP ←] Raw body ({responseJson.Length} chars): {responseJson}");

                // ۳) parse پاسخ
                using JsonDocument document = JsonDocument.Parse(responseJson);

                // usage
                if (document.RootElement.TryGetProperty("usage", out JsonElement usage) &&
                    usage.ValueKind == JsonValueKind.Object)
                {
                    if (usage.TryGetProperty("prompt_tokens", out JsonElement pt) && pt.TryGetInt32(out int p))
                        request.LastPromptTokens = p;

                    if (usage.TryGetProperty("completion_tokens", out JsonElement ct) && ct.TryGetInt32(out int c))
                        request.LastCompletionTokens = c;

                    onDebugLog?.Invoke($"[usage] prompt={request.LastPromptTokens} completion={request.LastCompletionTokens}");
                }

                var choice = document.RootElement.GetProperty("choices")[0];
                var message = choice.GetProperty("message");

                // ۴) آیا tool_calls دارد؟
                if (!message.TryGetProperty("tool_calls", out JsonElement toolCalls) ||
                    toolCalls.ValueKind != JsonValueKind.Array ||
                    toolCalls.GetArrayLength() == 0)
                {
                    // پاسخ نهایی
                    string finalAnswer = message.TryGetProperty("content", out JsonElement contentEl)
                        ? contentEl.GetString() ?? ""
                        : "";

                    onDebugLog?.Invoke($"[tools] finished at step {step + 1} (no more tool calls)");
                    return string.IsNullOrWhiteSpace(finalAnswer)
                        ? "⚠️ No response received."
                        : finalAnswer;
                }

                // ۵) پیام assistant با tool_calls را به تاریخچه اضافه کن
                //    (این پیام باید به‌عنوان assistant در messages بعدی باشد)
                request.Messages.Add(BuildAssistantToolCallMessage(message));

                // ۶) هر tool_call را اجرا کن
                foreach (JsonElement toolCall in toolCalls.EnumerateArray())
                {
                    string callId = toolCall.GetProperty("id").GetString() ?? "";
                    var function = toolCall.GetProperty("function");
                    string name = function.GetProperty("name").GetString() ?? "";
                    string args = function.TryGetProperty("arguments", out JsonElement argsEl)
                        ? argsEl.GetString() ?? "{}"
                        : "{}";

                    onDebugLog?.Invoke($"[tool-call] {name}({args})");

                    // اجرا
                    string result;
                    ToolDefinition tool = ToolRegistry.Get(name);
                    if (tool == null)
                    {
                        result = $"Error: tool '{name}' not found.";
                        onDebugLog?.Invoke($"[tool-call] ERROR: tool not found");
                    }
                    else
                    {
                        try
                        {
                            result = tool.Execute(args) ?? "";
                            onDebugLog?.Invoke($"[tool-result] {TruncateForLog(result, 120)}");
                        }
                        catch (Exception ex)
                        {
                            result = $"Error: {ex.Message}";
                            onDebugLog?.Invoke($"[tool-call] EXCEPTION: {ex.Message}");
                        }
                    }

                    // ۷) پیام tool را به messages اضافه کن
                    request.Messages.Add(new ChatMessage
                    {
                        Role = "tool",
                        Content = result,
                        ToolCallId = callId
                    });
                }
            }

            onDebugLog?.Invoke($"[tools] WARNING: reached maxSteps ({maxSteps}) without final answer.");
            return "⚠️ Reached maximum tool-calling steps without a final answer.";
        }



        // ---------------------------------------------------------
        // آماده‌سازی HttpClient
        // ---------------------------------------------------------
        private async Task<HttpClient> PrepareHttpClientAsync(LLM model)
        {
            string apiKey = model.GetApiKey();

            if (!string.IsNullOrEmpty(model.ApiKeyEnvironmentVariable) && string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException(
                    $"Environment variable '{model.ApiKeyEnvironmentVariable}' is not set.");

            HttpClient client;
            if (model.UseProxy && !string.IsNullOrEmpty(model.ProxyHost))
            {
                var proxy = new System.Net.WebProxy($"{model.ProxyHost}:{model.ProxyPort}");
                var handler = new HttpClientHandler { Proxy = proxy };
                client = new HttpClient(handler);
            }
            else
            {
                var handler = new HttpClientHandler { UseProxy = false };
                client = new HttpClient(handler);
            }

            client.Timeout = TimeSpan.FromSeconds(model.TimeoutSeconds);

            client.DefaultRequestHeaders.Clear();
            if (!string.IsNullOrEmpty(apiKey))
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            return client;
        }

        // ---------------------------------------------------------
        // ساخت درخواست OpenAI
        // ---------------------------------------------------------
        private HttpRequestMessage CreateRequest(ChatRequest request, bool isStream)
        {
            var requestData = new
            {
                model = request.Model.Name,
                messages = request.Messages.Select(m => new { role = m.Role, content = m.Content }),
                stream = isStream,
                temperature = request.Temperature,
                max_tokens = request.MaxTokens,
                top_p = request.TopP,
                stream_options = isStream ? new { include_usage = true } : null,
            };

            var jsonOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            string json = JsonSerializer.Serialize(requestData, jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return new HttpRequestMessage(HttpMethod.Post, request.Model.Endpoint)
            {
                Content = content
            };
        }


        private HttpRequestMessage CreateToolRequest(ChatRequest request)
        {
            var tools = new List<object>();
            foreach (ToolDefinition t in request.Tools)
            {
                tools.Add(new
                {
                    type = "function",
                    function = new
                    {
                        name = t.Name,
                        description = t.Description,
                        parameters = JsonSerializer.Deserialize<JsonElement>(t.ParametersJson)
                    }
                });
            }

            var requestData = new
            {
                model = request.Model.Name,
                messages = request.Messages.Select(BuildMessageForApi).ToList(),
                stream = false,
                temperature = request.Temperature,
                max_tokens = request.MaxTokens,
                tools = tools,
                tool_choice = "auto"
            };

            var jsonOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            string json = JsonSerializer.Serialize(requestData, jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return new HttpRequestMessage(HttpMethod.Post, request.Model.Endpoint)
            {
                Content = content
            };
        }

        private static object BuildMessageForApi(ChatMessage m)
        {
            if (m.Role == "tool")
            {
                return new
                {
                    role = "tool",
                    content = m.Content,
                    tool_call_id = m.ToolCallId
                };
            }

            if (m.Role == "assistant" && m.ToolCalls != null && m.ToolCalls.Count > 0)
            {
                return new
                {
                    role = "assistant",
                    content = m.Content,
                    tool_calls = m.ToolCalls
                };
            }

            return new
            {
                role = m.Role,
                content = m.Content
            };
        }

        private static ChatMessage BuildAssistantToolCallMessage(JsonElement message)
        {
            var msg = new ChatMessage
            {
                Role = "assistant",
                Content = message.TryGetProperty("content", out JsonElement c) ? c.GetString() ?? "" : ""
            };

            if (message.TryGetProperty("tool_calls", out JsonElement toolCalls))
            {
                foreach (JsonElement tc in toolCalls.EnumerateArray())
                {
                    var fn = tc.GetProperty("function");
                    msg.ToolCalls.Add(new ToolCall
                    {
                        Id = tc.GetProperty("id").GetString() ?? "",
                        Type = "function",
                        Function = new ToolCallFunction
                        {
                            Name = fn.GetProperty("name").GetString() ?? "",
                            Arguments = fn.TryGetProperty("arguments", out JsonElement a) ? a.GetString() ?? "{}" : "{}"
                        }
                    });
                }
            }

            return msg;
        }


        // ---------------------------------------------------------
        // ارسال عادی (non-stream)
        // ---------------------------------------------------------
        private async Task<string> SendNormalMessageAsync(
            HttpClient client,
            ChatRequest request,
            Action<string> onDebugLog,
            Action<string> onVerboseLog,
            DebugLevel debugLevel,
            CancellationToken cancellationToken)
        {
            using var httpRequest = CreateRequest(request, false);

            // Normal
            onDebugLog?.Invoke($"[HTTP →] POST {request.Model.Endpoint}");

            // Verbose
            if (debugLevel == DebugLevel.Verbose)
                onVerboseLog?.Invoke($"[HTTP →] Body: {await SafeReadContentAsync(httpRequest.Content)}");

            var sw = System.Diagnostics.Stopwatch.StartNew();
            HttpResponseMessage response = await client.SendAsync(httpRequest, cancellationToken);
            sw.Stop();

            // Normal
            onDebugLog?.Invoke($"[HTTP ←] Status: {(int)response.StatusCode} {response.StatusCode}  ({sw.ElapsedMilliseconds} ms)");
            onDebugLog?.Invoke($"[HTTP ←] Headers: {FormatHeaders(response)}");

            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            // Verbose
            if (debugLevel == DebugLevel.Verbose)
                onVerboseLog?.Invoke($"[HTTP ←] Raw body ({responseJson.Length} chars): {responseJson}");

            using JsonDocument document = JsonDocument.Parse(responseJson);

            string answer = document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (document.RootElement.TryGetProperty("usage", out JsonElement usage) && usage.ValueKind == JsonValueKind.Object)
            {
                if (usage.TryGetProperty("prompt_tokens", out JsonElement pt) && pt.TryGetInt32(out int p))
                    request.LastPromptTokens = p;

                if (usage.TryGetProperty("completion_tokens", out JsonElement ct) && ct.TryGetInt32(out int c))
                    request.LastCompletionTokens = c;

                onDebugLog?.Invoke($"[usage] prompt={request.LastPromptTokens} completion={request.LastCompletionTokens}");
            }
            else
            {
                onDebugLog?.Invoke("[usage] not provided by provider");
            }

            return answer ?? "⚠️ No response received.";
        }

        // ---------------------------------------------------------
        // ارسال استریم
        // ---------------------------------------------------------
        private async Task<string> SendStreamMessageAsync(
            HttpClient client,
            ChatRequest request,
            Action<string> onChunkReceived,
            Action<string> onDebugLog,
            Action<string> onVerboseLog,
            DebugLevel debugLevel,
            CancellationToken cancellationToken)
        {
            client.Timeout = TimeSpan.FromSeconds(request.Model.TimeoutSeconds * 2);

            using var httpRequest = CreateRequest(request, true);

            // Normal
            onDebugLog?.Invoke($"[HTTP →] POST {request.Model.Endpoint}  (stream=true)");

            // Verbose
            if (debugLevel == DebugLevel.Verbose)
                onVerboseLog?.Invoke($"[HTTP →] Body: {await SafeReadContentAsync(httpRequest.Content)}");

            var sw = System.Diagnostics.Stopwatch.StartNew();
            using HttpResponseMessage response = await client.SendAsync(
                httpRequest,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken
            );

            // Normal
            onDebugLog?.Invoke($"[HTTP ←] Status: {(int)response.StatusCode} {response.StatusCode}  ({sw.ElapsedMilliseconds} ms)");
            onDebugLog?.Invoke($"[HTTP ←] Headers: {FormatHeaders(response)}");

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            StringBuilder fullAnswer = new StringBuilder();
            string line;
            int chunkCount = 0;
            long firstChunkMs = -1;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Verbose: هر خط SSE خام
                if (debugLevel == DebugLevel.Verbose)
                    onVerboseLog?.Invoke($"[SSE ←] {line}");

                if (line.StartsWith("data: "))
                {
                    string jsonData = line.Substring(6).Trim();

                    if (jsonData == "[DONE]")
                    {
                        onDebugLog?.Invoke($"[SSE ←] [DONE] received.");
                        break;
                    }

                    try
                    {
                        using JsonDocument document = JsonDocument.Parse(jsonData);

                        if (document.RootElement.TryGetProperty("usage", out JsonElement usage) && usage.ValueKind == JsonValueKind.Object)
                        {
                            if (usage.TryGetProperty("prompt_tokens", out JsonElement pt) && pt.TryGetInt32(out int p))
                                request.LastPromptTokens = p;

                            if (usage.TryGetProperty("completion_tokens", out JsonElement ct) && ct.TryGetInt32(out int c))
                                request.LastCompletionTokens = c;

                            onDebugLog?.Invoke($"[usage] prompt={request.LastPromptTokens} completion={request.LastCompletionTokens}");
                        }

                        if (document.RootElement.TryGetProperty("choices", out JsonElement choices) && choices.GetArrayLength() > 0)
                        {
                            var choice = choices[0];

                            if (choice.TryGetProperty("delta", out JsonElement delta) &&
                                delta.TryGetProperty("content", out JsonElement contentElement))
                            {
                                string chunk = contentElement.GetString();
                                if (!string.IsNullOrEmpty(chunk))
                                {
                                    chunkCount++;

                                    if (firstChunkMs < 0)
                                    {
                                        firstChunkMs = sw.ElapsedMilliseconds;
                                        onDebugLog?.Invoke($"[stream] ⏱ First token at {firstChunkMs} ms");
                                    }

                                    fullAnswer.Append(chunk);

                                    // Verbose: هر chunk
                                    if (debugLevel == DebugLevel.Verbose)
                                        onVerboseLog?.Invoke($"[chunk #{chunkCount}] (+{chunk.Length} chars, total {fullAnswer.Length}) {TruncateForLog(chunk, 80)}");

                                    onChunkReceived?.Invoke(fullAnswer.ToString());
                                }
                            }
                        }
                    }
                    catch (JsonException jex)
                    {
                        onDebugLog?.Invoke($"[warn] JSON parse failed: {jex.Message}");
                        continue;
                    }
                }
            }

            sw.Stop();
            onDebugLog?.Invoke($"[stream] ✔ Finished. chunks={chunkCount}, chars={fullAnswer.Length}, total={sw.ElapsedMilliseconds} ms");

            return fullAnswer.Length > 0
                ? fullAnswer.ToString()
                : "⚠️ No response received.";
        }

        // ---------------------------------------------------------
        // کمکی‌ها
        // ---------------------------------------------------------
        private static async Task<string> SafeReadContentAsync(HttpContent content)
        {
            if (content == null) return "(no content)";
            try { return await content.ReadAsStringAsync(); }
            catch { return "(unreadable)"; }
        }

        private static string FormatHeaders(HttpResponseMessage response)
        {
            var sb = new StringBuilder();
            foreach (var h in response.Headers)
                sb.Append($"{h.Key}={string.Join(",", h.Value)}; ");
            foreach (var h in response.Content.Headers)
                sb.Append($"{h.Key}={string.Join(",", h.Value)}; ");
            return sb.ToString().TrimEnd(' ', ';');
        }

        private static string TruncateForLog(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Length <= max ? s : s.Substring(0, max) + "…";
        }

        public string GetModelInfo(LLM model)
        {
            return $"Provider: {model.Provider}\n" +
                   $"Name: {model.Name}\n" +
                   $"Type: {model.Type}\n" +
                   $"Endpoint: {model.Endpoint}\n" +
                   $"Timeout: {model.TimeoutSeconds} sec\n" +
                   $"Temperature: {model.Temperature}\n" +
                   $"Max Tokens: {model.MaxTokens}\n" +
                   $"Context Window Tokens: {model.ContextWindowTokens}\n" +
                   $"Proxy: {(model.UseProxy ? $"Enabled > {model.ProxyHost}:{model.ProxyPort}" : "Disabled")}\n" +
                   $"Remarks: {model.Remarks}";
        }
    }
}