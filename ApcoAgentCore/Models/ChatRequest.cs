namespace ApcoAgentCore.Models
{
    public class ChatRequest
    {
        public LLM Model { get; set; } = new LLM();
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        public double Temperature { get; set; }
        public int MaxTokens { get; set; }
        public double? TopP { get; set; }

        public bool UseStream { get; set; }

        public int? LastPromptTokens { get; set; }
        public int? LastCompletionTokens { get; set; }

        public List<ToolDefinition> Tools { get; set; } = new List<ToolDefinition>();

    }
}