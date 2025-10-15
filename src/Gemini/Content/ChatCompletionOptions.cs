namespace Gemini.Content;

public class ChatCompletionOptions
{
    public double? Temperature { get; set; }
    public double? TopP { get; set; }
    public int? TopK { get; set; }
    public int? MaxTokens { get; set; }
    public List<string>? StopSequences { get; set; }
}