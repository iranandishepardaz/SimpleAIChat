using System;

namespace ApcoAgentCore.Models
{
    /// <summary>
    /// تعریف یک ابزار که مدل می‌تواند صدا بزند.
    /// 
    /// مدل فقط Name، Description و ParametersJson را می‌بیند.
    /// Execute یک تابع محلی است که وقتی مدل ابزار را صدا زد، اجرا می‌شود.
    /// </summary>
    public class ToolDefinition
    {
        /// <summary>نام یکتای ابزار، مثل "get_current_time".</summary>
        public string Name { get; set; } = "";

        /// <summary>توضیح کوتاه برای مدل — مدل بر اساس این تصمیم می‌گیرد چه‌وقت از آن استفاده کند.</summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// JSON Schema پارامترها به فرمت OpenAI.
        /// مثال بدون پارامتر: {"type":"object","properties":{}}
        /// مثال با پارامتر: {"type":"object","properties":{"city":{"type":"string"}},"required":["city"]}
        /// </summary>
        public string ParametersJson { get; set; } = "{\"type\":\"object\",\"properties\":{}}";

        /// <summary>
        /// تابعی که وقتی مدل ابزار را صدا زد اجرا می‌شود.
        /// ورودی: رشته‌ی JSON آرگومان‌ها (از مدل). خروجی: رشته‌ی نتیجه.
        /// </summary>
        public Func<string, string> Execute { get; set; } = _ => "";
    }
}