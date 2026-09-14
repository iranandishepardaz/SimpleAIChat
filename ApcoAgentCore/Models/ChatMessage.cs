using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApcoAgentCore.Models
{
    public sealed class ChatMessage
    {   
        // ---------------------------------------------------------
        // مدل پیام
        // ---------------------------------------------------------
            public string Role { get; set; } = "";
            public string Content { get; set; } = "";

        public string ToolCallId { get; set; } = "";
        public List<ToolCall> ToolCalls { get; set; } = new List<ToolCall>();

    }
}
