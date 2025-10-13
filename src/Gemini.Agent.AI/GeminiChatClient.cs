using Microsoft.Extensions.AI;

namespace Gemini.Agent.AI;

public class GeminiChatClient : IChatClient
{
    private HttpClient client = null!;
    private string _modelName = "gemini-2.5-flash-lite";

    public GeminiChatClient()
    {
        client.BaseAddress = new Uri("");
    }

    public void Dispose()
    {
        client.Dispose();
    }

    public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        client = new HttpClient();
        var response = await client.GetAsync(new Uri(
                $"https://generativelanguage.googleapis.com/v1beta/models/{_modelName}:generateContent"),
            cancellationToken);

        return new ChatResponse(new ChatMessage(ChatRole.Assistant,
            await response.Content.ReadAsStringAsync(cancellationToken)));
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        client = new HttpClient();
        var response = await client.GetStreamAsync(new Uri(
                $"https://generativelanguage.googleapis.com/v1beta/models/{_modelName}:streamGenerateContent"),
            cancellationToken);

        //TODO: implement
        yield return null!;
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        throw new NotImplementedException();
    }
}