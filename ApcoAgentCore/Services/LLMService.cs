using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ApcoAgentCore.Models;

namespace ApcoAgentCore.Services
{
    public sealed class LLMService
    {
        // ---------------------------------------------------------
        // نام فایل تنظیمات مدل‌ها
        // ---------------------------------------------------------
        private const string FileName = "models.json";

              
        // ---------------------------------------------------------
        // خواندن مدل‌ها از فایل JSON
        //
        // خروجی این متد لیستی از اشیای LLM است.
        // ---------------------------------------------------------
        public List<LLM> LoadModels()
        {
            // C:\Users\Username\AppData\Roaming
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filePath = Path.Combine(appDataPath, "ApcoAgent", "SimpleAIChat", FileName);


            // اگر فایل وجود نداشته باشد، خطا ایجاد می‌کنیم.
            if (!File.Exists(filePath))
                throw new FileNotFoundException("فایل مدل‌ها پیدا نشد.", filePath);


            // -----------------------------------------------------
            // کل محتوای فایل JSON را می‌خوانیم.
            // -----------------------------------------------------
            string json = File.ReadAllText(filePath);


            // -----------------------------------------------------
            // JSON را به List<LLM> تبدیل می‌کنیم.
            //
            // PropertyNameCaseInsensitive باعث می‌شود مثلاً
            // "Name" و "name" هر دو قابل خواندن باشند.
            // -----------------------------------------------------
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;


            List<LLM>? models = JsonSerializer.Deserialize<List<LLM>>(json, options);


            // اگر JSON خالی یا نامعتبر باشد،
            // یک لیست خالی برمی‌گردانیم.
            return models ?? new List<LLM>();
        }
    }
}