namespace Gemini.Content;

public class ChatMessage
{
    public string Role { get; set; }
    public List<ContentPart> Contents { get; set; }

    public ChatMessage(string role, string content)
    {
        Role = role;
        Contents = new List<ContentPart>
        {
            new TextContent(content)
        };
    }

    public ChatMessage(string role, IEnumerable<ContentPart> contents)
    {
        Role = role;
        Contents = contents.ToList();
    }

    public static ChatMessage CreateUserMessage(string content) => new("user", content);
    public static ChatMessage CreateAssistantMessage(string content) => new("assistant", content);
    public static ChatMessage CreateSystemMessage(string content) => new("system", content);
}