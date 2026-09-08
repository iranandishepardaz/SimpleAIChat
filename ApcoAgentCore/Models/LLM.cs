
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
    }


}
