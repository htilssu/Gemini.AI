using System.ClientModel;

namespace Gemini.Agent.AI;

public class ChatClient
{
    public ChatClient(string model, string apiKey) : this(model, new ApiKeyCredential(apiKey),
        new GeminiClientOptions())
    {
    }

    private ChatClient(string model, ApiKeyCredential apiKey, GeminiClientOptions geminiClientOptions)
    {
    }
}