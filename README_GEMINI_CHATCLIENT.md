# Gemini ChatClient

Implementation của ChatClient cho Google Gemini API, tương tự như OpenAI ChatClient.

## Features

✅ **Gemini ChatClient** - Client chính để tương tác với Gemini API
- Constructor: `new ChatClient(model, apiKey)`
- Non-streaming: `CompleteChatAsync()`
- Streaming: `CompleteChatStreamingAsync()`

✅ **Model Classes**
- `ChatMessage` - với constructor `new ChatMessage(ChatRole, content)`
- `ChatCompletion` - response từ non-streaming API
- `StreamingChatCompletionUpdate` - updates từ streaming API
- `ChatCompletionOptions` - options cho request (temperature, topP, maxTokens, etc.)

✅ **Multi-targeting Support**
- .NET 9.0
- .NET 8.0  
- .NET Standard 2.0

## Usage

### Basic Example

```csharp
using Gemini;

// Tạo client
var client = new ChatClient("gemini-pro", "your-api-key");

// Tạo messages
var messages = new List<ChatMessage>
{
    ChatMessage.CreateUserMessage("Hello, how are you?")
};

// Non-streaming chat completion
var response = await client.CompleteChatAsync(messages);
Console.WriteLine(response.Content);
```

### With Options

```csharp
var options = new ChatCompletionOptions
{
    Temperature = 0.7,
    MaxTokens = 1000,
    TopP = 0.9
};

var response = await client.CompleteChatAsync(messages, options);
```

### Streaming Example

```csharp
await foreach (var update in client.CompleteChatStreamingAsync(messages))
{
    Console.Write(update.ContentUpdate);
}
```

### With Custom Endpoint

```csharp
var options = new GeminiClientOptions
{
    Endpoint = new Uri("https://custom-endpoint.com")
};

var client = new ChatClient("gemini-pro", apiKey, options);
```

## API Reference

### ChatClient

**Constructors:**
- `ChatClient(string model, string apiKey)`
- `ChatClient(string model, ApiKeyCredential apiKey)`
- `ChatClient(string model, ApiKeyCredential apiKey, GeminiClientOptions options)`

**Methods:**
- `Task<ChatCompletion> CompleteChatAsync(IEnumerable<ChatMessage> messages, ChatCompletionOptions? options = null, CancellationToken cancellationToken = default)`
- `IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteChatStreamingAsync(IEnumerable<ChatMessage> messages, ChatCompletionOptions? options = null, CancellationToken cancellationToken = default)`

### ChatMessage

**Constructors:**
- `ChatMessage(string role, string content)`
- `ChatMessage(string role, IEnumerable<ContentPart> contents)`

**Static Factory Methods:**
- `ChatMessage.CreateUserMessage(string content)`
- `ChatMessage.CreateAssistantMessage(string content)`
- `ChatMessage.CreateSystemMessage(string content)`

### ChatCompletionOptions

**Properties:**
- `double? Temperature` - Controls randomness (0.0 to 1.0)
- `double? TopP` - Nucleus sampling threshold
- `int? TopK` - Top-K sampling parameter
- `int? MaxTokens` - Maximum tokens in response
- `List<string>? StopSequences` - Sequences where API will stop generating

## Notes

- Gemini API không có system role riêng biệt, system messages sẽ được map thành user messages
- Role "assistant" được map thành "model" khi gửi request đến Gemini API
- Default endpoint: `https://generativelanguage.googleapis.com`

## Build

```bash
dotnet build src/Gemini/Gemini.csproj
```

## License

[Your License Here]
