namespace ApcoAgentCore.Models
{
    public class Preset
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string SystemPrompt { get; set; } = "";
        public string KnowledgeFile { get; set; } = "";

        public double? Temperature { get; set; }
        public int? MaxTokens { get; set; }
        public double? TopP { get; set; }

        public string OutputFormat { get; set; } = "text";

        public static string DefaultPresetsJson => @"[
  {
    ""Id"": ""fa-chat"",
    ""Name"": ""چت فارسی"",
    ""SystemPrompt"": ""تو یک دستیار فارسی‌زبان هستی. پاسخ‌ها را روان، دقیق و کوتاه به فارسی بنویس."",
    ""KnowledgeFile"": """",
    ""Temperature"": 0.7,
    ""MaxTokens"": null,
    ""TopP"": null
  },
  {
    ""Id"": ""code-csharp"",
    ""Name"": ""کد C#"",
    ""SystemPrompt"": ""You are a senior C# developer. Answer with clean, idiomatic C# code and short explanations. Prefer .NET conventions and nullable reference types."",
    ""KnowledgeFile"": """",
    ""Temperature"": 0.2,
    ""MaxTokens"": null,
    ""TopP"": null
  },
  {
    ""Id"": ""code-python"",
    ""Name"": ""کد Python"",
    ""SystemPrompt"": ""You are a senior Python developer. Answer with clean, idiomatic Python (PEP 8). Prefer standard library when reasonable."",
    ""KnowledgeFile"": """",
    ""Temperature"": 0.2,
    ""MaxTokens"": null,
    ""TopP"": null
  },
  {
    ""Id"": ""customer-support"",
    ""Name"": ""پاسخگوی مشتری"",
    ""SystemPrompt"": ""تو یک پاسخگوی مشتری هستی. فقط بر اساس اطلاعات دانشنامه پاسخ بده. اگر پاسخ در دانشنامه نبود، صادقانه بگو که اطلاعات کافی نداری."",
    ""KnowledgeFile"": """",
    ""Temperature"": 0.3,
    ""MaxTokens"": null,
    ""TopP"": null
  }
]";
    }
}