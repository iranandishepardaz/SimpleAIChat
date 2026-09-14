using System;
using System.Collections.Generic;
using ApcoAgentCore.Models;

namespace ApcoAgentCore.Services
{
    /// <summary>
    /// فهرست ابزارهای موجود.
    /// فعلاً فقط یک ابزار دارد: get_current_time.
    /// </summary>
    public static class ToolRegistry
    {
        private static readonly List<ToolDefinition> _tools = new List<ToolDefinition>
        {
            new ToolDefinition
            {
                Name = "get_current_time",
                Description = "Returns the current local time in HH:mm:ss format. Use this whenever the user asks about the current time.",
                ParametersJson = "{\"type\":\"object\",\"properties\":{}}",
                Execute = _ => DateTime.Now.ToString("HH:mm:ss")
            }
        };

        /// <summary>همه‌ی ابزارهای موجود.</summary>
        public static List<ToolDefinition> GetAll()
        {
            return _tools;
        }

        /// <summary>یک ابزار با نام مشخص. اگر پیدا نشد، null.</summary>
        public static ToolDefinition Get(string name)
        {
            return _tools.Find(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
        }
    }
}