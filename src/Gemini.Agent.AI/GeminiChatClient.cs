using System.Runtime.CompilerServices;
using Gemini.Content;
using Gemini.Content.Agent.AI.Extensions;
using Microsoft.Extensions.AI;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace Gemini.Agent.AI;

public class GeminiChatClient : IChatClient
{
    private readonly ChatClient _client;

    public ChatClientMetadata Metadata { get; }

    public GeminiChatClient(ChatClient chatClient, string? modelId = null)
    {
        _client = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
        Metadata = new ChatClientMetadata(providerName: "Gemini", defaultModelId: modelId);
    }

    public void Dispose()
    {
    }

    public async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var geminiMessages = ConvertToGeminiMessages(messages);
        var geminiOptions = ConvertToGeminiOptions(options);

        var completion = await _client.CompleteChatAsync(geminiMessages, geminiOptions, cancellationToken)
            .ConfigureAwait(false);

        return new ChatResponse(
            new ChatMessage(ChatRole.Assistant, completion.Content));
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var geminiMessages = ConvertToGeminiMessages(messages);
        var geminiOptions = ConvertToGeminiOptions(options);

        await foreach (var update in _client
                           .CompleteChatStreamingAsync(geminiMessages, geminiOptions, cancellationToken)
                           .ConfigureAwait(false))
        {
            yield return new ChatResponseUpdate(ChatRole.Assistant, update.ContentUpdate);
        }
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        return serviceType == typeof(ChatClient) ? _client : null;
    }

    public TService? GetService<TService>(object? key = null) where TService : class
    {
        return GetService(typeof(TService), key) as TService;
    }

    private IEnumerable<Content.ChatMessage> ConvertToGeminiMessages(
        IEnumerable<ChatMessage> messages)
    {
        return messages.Select(message => message.ToGeminiChatMessage());
    }

    private ChatCompletionOptions? ConvertToGeminiOptions(ChatOptions? options)
    {
        if (options == null)
            return null;

        return new ChatCompletionOptions
        {
            Temperature = options.Temperature,
            TopP = options.TopP,
            MaxTokens = options.MaxOutputTokens,
            StopSequences = options.StopSequences?.ToList()
        };
    }
}