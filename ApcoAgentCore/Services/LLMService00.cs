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
    public class LLMService00
    {
      
        public LLMService00()
        {

        }

        // ---------------------------------------------------------
        // بارگذاری مدل‌ها از فایل JSON
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
            {
                models = models.FindAll(m => m.Enabled);
            }
            return models ?? new List<LLM>();
        }

        // ---------------------------------------------------------
        // متد اصلی برای ارسال پیام به مدل
        // ---------------------------------------------------------
        public async Task<string> SendMessageAsync(
            LLM model,
            string prompt,
            bool useStream = false,
            Action<string> onChunkReceived = null,
            Action<string> onDebugLog = null,
            CancellationToken cancellationToken = default)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be empty", nameof(prompt));

            onDebugLog?.Invoke($"[Service] Preparing HttpClient...");
            using var client = await PrepareHttpClientAsync(model);

            onDebugLog?.Invoke($"[Service] Timeout = {model.TimeoutSeconds}s | Endpoint = {model.Endpoint}");

            if (useStream)
            {
                return await SendStreamMessageAsync(client, model, prompt, onChunkReceived, onDebugLog, cancellationToken);
            }
            else
            {
                return await SendNormalMessageAsync(client, model, prompt, onDebugLog, cancellationToken);
            }
        }

        // ---------------------------------------------------------
        // آماده‌سازی HttpClient با تنظیمات پروکسی و API Key
        // ---------------------------------------------------------
        private async Task<HttpClient> PrepareHttpClientAsync(LLM model)
        {
            string apiKey = model.GetApiKey();

            if (!string.IsNullOrEmpty(model.ApiKeyEnvironmentVariable) && string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException(
                    $"Environment variable '{model.ApiKeyEnvironmentVariable}' is not set."
                );
            }

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
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            }

            return client;
        }

        // ---------------------------------------------------------
        // ساخت درخواست به فرمت OpenAI
        // ---------------------------------------------------------
        private HttpRequestMessage CreateRequest(LLM model, string prompt, bool isStream)
        {
            var requestData = new
            {
                model = model.Name,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                stream = isStream,
                temperature = model.Temperature,
                max_tokens = model.MaxTokens
            };

            string json = JsonSerializer.Serialize(requestData);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            return new HttpRequestMessage(HttpMethod.Post, model.Endpoint)
            {
                Content = content
            };
        }

        // ---------------------------------------------------------
        // ارسال پیام به حالت عادی (بدون استریم)
        // ---------------------------------------------------------
        private async Task<string> SendNormalMessageAsync(
            HttpClient client,
            LLM model,
            string prompt,
            Action<string> onDebugLog,
            CancellationToken cancellationToken)
        {
            using var request = CreateRequest(model, prompt, false);

            // لاگ درخواست خام
            onDebugLog?.Invoke($"[HTTP →] POST {model.Endpoint}");
            onDebugLog?.Invoke($"[HTTP →] Body: {await SafeReadContentAsync(request.Content)}");

            var sw = System.Diagnostics.Stopwatch.StartNew();
            HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
            sw.Stop();

            onDebugLog?.Invoke($"[HTTP ←] Status: {(int)response.StatusCode} {response.StatusCode}  ({sw.ElapsedMilliseconds} ms)");
            onDebugLog?.Invoke($"[HTTP ←] Headers: {FormatHeaders(response)}");

            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            onDebugLog?.Invoke($"[HTTP ←] Raw body ({responseJson.Length} chars): {responseJson}");

            using JsonDocument document = JsonDocument.Parse(responseJson);

            string answer = document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return answer ?? "⚠️ No response received.";
        }

        // ---------------------------------------------------------
        // ارسال پیام به حالت استریم
        // ---------------------------------------------------------
        private async Task<string> SendStreamMessageAsync(
            HttpClient client,
            LLM model,
            string prompt,
            Action<string> onChunkReceived,
            Action<string> onDebugLog,
            CancellationToken cancellationToken)
        {
            client.Timeout = TimeSpan.FromSeconds(model.TimeoutSeconds * 2);

            using var request = CreateRequest(model, prompt, true);

            onDebugLog?.Invoke($"[HTTP →] POST {model.Endpoint}  (stream=true)");
            onDebugLog?.Invoke($"[HTTP →] Body: {await SafeReadContentAsync(request.Content)}");

            var sw = System.Diagnostics.Stopwatch.StartNew();
            using HttpResponseMessage response = await client.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken
            );

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

                // لاگ خط خام SSE
                onDebugLog?.Invoke($"[SSE ←] {line}");

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

                        if (document.RootElement.TryGetProperty("choices", out JsonElement choices) &&
                            choices.GetArrayLength() > 0)
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

                                    onDebugLog?.Invoke($"[chunk #{chunkCount}] (+{chunk.Length} chars, total {fullAnswer.Length}) {TruncateForLog(chunk, 80)}");

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
        // متدهای کمکی برای لاگ
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

        // ---------------------------------------------------------
        // متد کمکی برای دریافت اطلاعات مدل
        // ---------------------------------------------------------
        public string GetModelInfo(LLM model)
        {
            return $"Provider: {model.Provider}\n" +
                   $"Name: {model.Name}\n" +
                   $"Type: {model.Type}\n" +
                   $"Endpoint: {model.Endpoint}\n" +
                   $"Timeout: {model.TimeoutSeconds} sec\n" +
                   $"Temperature: {model.Temperature}\n" +
                   $"Max Tokens: {model.MaxTokens}\n" +
                   $"Proxy: {(model.UseProxy ? $"Enabled > {model.ProxyHost}:{model.ProxyPort}" : "Disabled")}\n" +
                   $"Remarks: {model.Remarks}";
        }
    }
}