using Microsoft.Extensions.AI;

namespace Gemini.Content.Agent.AI.Extensions;

public static class ChatMessageExtension
{
    public static ChatMessage ToGeminiChatMessage(this Microsoft.Extensions.AI.ChatMessage chatMessage)
    {
        var role = chatMessage.Role switch
        {
            _ => "user"
        };

        var contents = chatMessage.Contents.Select(content => content.ToGeminiContent()).ToList();
        return new ChatMessage(role, contents);
    }
}