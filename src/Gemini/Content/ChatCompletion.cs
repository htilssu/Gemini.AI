using System.Text.Json;

namespace Gemini.Content;

public class ChatCompletion
{
    public string Content { get; }
    public string? FinishReason { get; }
    public JsonElement? UsageMetadata { get; }
    public JsonElement? RawResponse { get; }

    public ChatCompletion(string content, string? finishReason, JsonElement? usageMetadata, JsonElement? rawResponse)
    {
        Content = content;
        FinishReason = finishReason;
        UsageMetadata = usageMetadata;
        RawResponse = rawResponse;
    }

    public override string ToString() => Content;
}