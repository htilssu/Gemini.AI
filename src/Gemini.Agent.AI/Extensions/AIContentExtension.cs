using Microsoft.Extensions.AI;

namespace Gemini.Content.Agent.AI.Extensions;

public static class AIContentExtension
{
    public static ContentPart ToGeminiContent(this AIContent content)
    {
        //TODO: more content type
        return new TextContent(content.ToString()!);
    }
}