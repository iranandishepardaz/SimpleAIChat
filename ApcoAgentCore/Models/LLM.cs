
using ApcoAgentCore.Services;
using System.Text;
using System.Text.Json;

namespace ApcoAgentCore.Models
{
    public sealed class LLM
    {
        public string Id { get; set; } = "";

        public string Name { get; set; } = "";

        public string Provider { get; set; } = "";

        public string Type { get; set; } = "Cloud";

        public string Endpoint { get; set; } = "";

        // نام Environment Variable که API Key داخل آن قرار دارد.
        // خود API Key هرگز داخل فایل ذخیره نمی‌شود.
        public string ApiKeyEnvironmentVariable { get; set; } = "";

        public int TimeoutSeconds { get; set; } = 120;

        public double Temperature { get; set; } = 0.0;

        public int MaxTokens { get; set; } = 500;

        public int ContextWindowTokens { get; set; } = 8192;
        public bool SupportsTools { get; set; } = false;

        public bool Enabled { get; set; } = true;

        public bool UseProxy { get; set; } = false;

        public string ProxyHost { get; set; } = "127.0.0.1";

        public int ProxyPort { get; set; } = 1088;

        public string Remarks { get; set; } = "";

        public Dictionary<string, object>? ExtraOptions { get; set; }

        public string GetApiKey()
        {
            if (string.IsNullOrWhiteSpace(ApiKeyEnvironmentVariable))
                return "";

            return Environment.GetEnvironmentVariable(ApiKeyEnvironmentVariable) ?? "";
        }



        public const string DefaultModelsJson = @"[
          {
            ""Id"": ""Local Ollama-gemma2-2b"",
            ""Name"": ""gemma2:2b"",
            ""Provider"": ""Ollama"",
            ""Type"": ""Local"",
            ""Endpoint"": ""http://localhost:11434/v1/chat/completions"",
            ""ApiKeyEnvironmentVariable"": """",
            ""TimeoutSeconds"": 300,
            ""Temperature"": 0,
            ""MaxTokens"": 1024,
            ""Enabled"": true,
            ""UseProxy"": false,
            ""ProxyHost"": ""127.0.0.1"",
            ""ProxyPort"": 1088,
            ""ExtraOptions"": null,
            ""Remarks"": ""سرعت بالا و کیفیت مناسب برای چت عمومی سبک""
          },
          {
            ""Id"": ""Local Ollama-gemma3-1b"",
            ""Name"": ""gemma3:1b"",
            ""Provider"": ""Ollama"",
            ""Type"": ""Local"",
            ""Endpoint"": ""http://localhost:11434/v1/chat/completions"",
            ""ApiKeyEnvironmentVariable"": """",
            ""TimeoutSeconds"": 300,
            ""Temperature"": 0,
            ""MaxTokens"": 1024,
            ""Enabled"": true,
            ""UseProxy"": false,
            ""ProxyHost"": ""127.0.0.1"",
            ""ProxyPort"": 1088,
            ""ExtraOptions"": null,
            ""Remarks"": ""سبک و سریع برای تست و پاسخ‌های کوتاه""
          },
          {
            ""Id"": ""Local  Ollama-gemma3-4b"",
            ""Name"": ""gemma3-4b"",
            ""Provider"": ""Ollama"",
            ""Type"": ""Local"",
            ""Endpoint"": ""http://localhost:11434/v1/chat/completions"",
            ""ApiKeyEnvironmentVariable"": """",
            ""TimeoutSeconds"": 300,
            ""Temperature"": 0,
            ""MaxTokens"": 1024,
            ""Enabled"": true,
            ""UseProxy"": false,
            ""ProxyHost"": ""127.0.0.1"",
            ""ProxyPort"": 1088,
            ""ExtraOptions"": null,
            ""Remarks"": ""تعادل خوب بین سرعت و کیفیت برای چت عمومی""
          }
        ]";
    }
}