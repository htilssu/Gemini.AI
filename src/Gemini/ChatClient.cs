using System.ClientModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gemini.Content;

public class ChatClient
{
    private readonly string _model;
    private readonly string _apiKey;
    private readonly GeminiClientOptions _options;
    private readonly HttpClient _httpClient;
    private const string DefaultEndpoint = "https://generativelanguage.googleapis.com";

    public ChatClient(string model, string apiKey) : this(model, new ApiKeyCredential(apiKey),
        new GeminiClientOptions())
    {
    }

    public ChatClient(string model, ApiKeyCredential apiKey) : this(model, apiKey, new GeminiClientOptions())
    {
    }

    public ChatClient(string model, ApiKeyCredential apiKey, GeminiClientOptions options)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
        if (apiKey == null) throw new ArgumentNullException(nameof(apiKey));

        // Extract the key from ApiKeyCredential - we need to store it as string
        _apiKey = ExtractApiKey(apiKey);
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpClient = new HttpClient();

        if (_options.Endpoint == null)
        {
            _options.Endpoint = new Uri(DefaultEndpoint);
        }
    }
    
    private static string ExtractApiKey(ApiKeyCredential credential)
    {
        var field = typeof(ApiKeyCredential).GetField("_key", BindingFlags.NonPublic | BindingFlags.Instance);
        return field?.GetValue(credential) as string ??
               throw new InvalidOperationException("Unable to extract API key");
    }

    public async Task<ChatCompletion> CompleteChatAsync(IEnumerable<ChatMessage> messages,
        ChatCompletionOptions? options = null, CancellationToken cancellationToken = default)
    {
        var requestBody = BuildRequestBody(messages, options, stream: false);
        var endpoint = $"{_options.Endpoint}/v1beta/models/{_model}:generateContent?key={_apiKey}";

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

#if NETSTANDARD2_0
        var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#else
        var responseContent =
            await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#endif
        return ParseChatCompletion(responseContent);
    }

    public async IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteChatStreamingAsync(
        IEnumerable<ChatMessage> messages,
        ChatCompletionOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var requestBody = BuildRequestBody(messages, options, stream: true);
        var endpoint =
            $"{_options.Endpoint}/v1beta/models/{_model}:streamGenerateContent?key={_apiKey}&alt=sse";

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
        };

        using var response = await _httpClient
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

#if NETSTANDARD2_0
        using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
#else
        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
#endif
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (line.StartsWith("data: "))
            {
                var data = line.Substring(6);
                if (data.Trim() == "[DONE]")
                    break;

                var update = ParseStreamingUpdate(data);
                if (update != null)
                    yield return update;
            }
        }
    }

    private string BuildRequestBody(IEnumerable<ChatMessage> messages, ChatCompletionOptions? options,
        bool stream)
    {
        var contents = new List<object>();

        foreach (var message in messages)
        {
            var role = message.Role switch
            {
                "user" => "user",
                "assistant" => "model",
                "system" => "user", // Gemini doesn't have system role, map to user
                _ => "user"
            };

            var parts = new List<object>();

            foreach (var content in message.Contents)
            {
                if (content is TextContent textContent)
                {
                    parts.Add(new
                    {
                        text = textContent.Text
                    });
                }
            }

            contents.Add(new
            {
                role, parts
            });
        }

        var requestObj = new
        {
            contents,
            generationConfig = new
            {
                temperature = options?.Temperature,
                topP = options?.TopP,
                topK = options?.TopK,
                maxOutputTokens = options?.MaxTokens,
                stopSequences = options?.StopSequences
            }
        };

        var jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return JsonSerializer.Serialize(requestObj, jsonOptions);
    }

    private ChatCompletion ParseChatCompletion(string responseContent)
    {
        var jsonDoc = JsonDocument.Parse(responseContent);
        var root = jsonDoc.RootElement;

        var candidates = root.GetProperty("candidates");
        if (candidates.GetArrayLength() == 0)
        {
            return new ChatCompletion(string.Empty, null, null, null);
        }

        var candidate = candidates[0];
        var content = candidate.GetProperty("content");
        var parts = content.GetProperty("parts");

        var textBuilder = new StringBuilder();
        foreach (var part in parts.EnumerateArray())
        {
            if (part.TryGetProperty("text", out var textProp))
            {
                textBuilder.Append(textProp.GetString());
            }
        }

        string? finishReason = null;
        if (candidate.TryGetProperty("finishReason", out var finishReasonProp))
        {
            finishReason = finishReasonProp.GetString();
        }

        JsonElement? usageMetadata = null;
        if (root.TryGetProperty("usageMetadata", out var usageProp))
        {
            usageMetadata = usageProp;
        }

        return new ChatCompletion(textBuilder.ToString(), finishReason, usageMetadata, root);
    }

    private StreamingChatCompletionUpdate? ParseStreamingUpdate(string data)
    {
        try
        {
            var jsonDoc = JsonDocument.Parse(data);
            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
            {
                return null;
            }

            var candidate = candidates[0];
            var content = candidate.GetProperty("content");
            var parts = content.GetProperty("parts");

            var textBuilder = new StringBuilder();
            foreach (var part in parts.EnumerateArray())
            {
                if (part.TryGetProperty("text", out var textProp))
                {
                    textBuilder.Append(textProp.GetString());
                }
            }

            string? finishReason = null;
            if (candidate.TryGetProperty("finishReason", out var finishReasonProp))
            {
                finishReason = finishReasonProp.GetString();
            }

            return new StreamingChatCompletionUpdate(textBuilder.ToString(), finishReason);
        }
        catch
        {
            return null;
        }
    }
}