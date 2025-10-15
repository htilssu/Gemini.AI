namespace Gemini.Content;

public class StreamingChatCompletionUpdate
{
    public string ContentUpdate { get; }
    public string? FinishReason { get; }

    public StreamingChatCompletionUpdate(string contentUpdate, string? finishReason)
    {
        ContentUpdate = contentUpdate;
        FinishReason = finishReason;
    }

    public override string ToString() => ContentUpdate;
}