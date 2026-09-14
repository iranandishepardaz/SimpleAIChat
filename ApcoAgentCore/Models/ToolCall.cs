namespace ApcoAgentCore.Models
{
    public class ToolCall
    {
        public string Id { get; set; } = "";
        public string Type { get; set; } = "function";
        public ToolCallFunction Function { get; set; } = new ToolCallFunction();
    }

    public class ToolCallFunction
    {
        public string Name { get; set; } = "";
        public string Arguments { get; set; } = "{}";
    }
}