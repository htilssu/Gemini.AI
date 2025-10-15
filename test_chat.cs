using Microsoft.Extensions.AI;

namespace TestNS
{
    public class TestChatClient : IChatClient
    {
        public ChatClientMetadata Metadata => new ChatClientMetadata();
        
        public void Dispose() { }
        
        public Task<ChatResponse> GetResponseAsync(
            IEnumerable<ChatMessage> messages,
            ChatOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new ChatResponse(new ChatMessage(ChatRole.Assistant, "Test")));
        }
        
        public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
            IEnumerable<ChatMessage> messages,
            ChatOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            yield return new ChatResponseUpdate { Text = "Test" };
        }
        
        public object? GetService(Type serviceType, object? serviceKey = null) => null;
        public TService? GetService<TService>(object? key = null) where TService : class => null;
    }
}
