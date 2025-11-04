using System.ClientModel;
using System.Reflection;
using System.Text.Json;
using FluentAssertions;
using Gemini.Content;

namespace Gemini.Test;

public class ChatClientInternalTests
{
    private const string TestApiKey = "test-api-key-12345";
    private const string TestModel = "gemini-pro";

    [Fact]
    public void ExtractApiKey_WithValidCredential_ShouldExtractKey()
    {
        // Arrange
        var apiKey = new ApiKeyCredential(TestApiKey);
        var chatClient = new ChatClient(TestModel, apiKey);
        var extractMethod = typeof(ChatClient).GetMethod("ExtractApiKey", 
            BindingFlags.NonPublic | BindingFlags.Static);

        // Act
        var result = extractMethod?.Invoke(null, new object[] { apiKey });

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(TestApiKey);
    }

    [Fact]
    public void BuildRequestBody_WithUserMessage_ShouldCreateValidJson()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var messages = new[] { ChatMessage.CreateUserMessage("Hello") };
        var buildMethod = typeof(ChatClient).GetMethod("BuildRequestBody",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = buildMethod?.Invoke(chatClient, new object[] { messages, null, false });

        // Assert
        result.Should().NotBeNull();
        var json = result as string;
        json.Should().NotBeNullOrEmpty();
        
        var doc = JsonDocument.Parse(json!);
        doc.RootElement.TryGetProperty("contents", out var contents).Should().BeTrue();
        contents.GetArrayLength().Should().Be(1);
    }

    [Fact]
    public void BuildRequestBody_WithAssistantMessage_ShouldMapToModelRole()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var messages = new[] { ChatMessage.CreateAssistantMessage("Response") };
        var buildMethod = typeof(ChatClient).GetMethod("BuildRequestBody",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = buildMethod?.Invoke(chatClient, new object[] { messages, null, false });

        // Assert
        result.Should().NotBeNull();
        var json = result as string;
        var doc = JsonDocument.Parse(json!);
        var role = doc.RootElement.GetProperty("contents")[0].GetProperty("role").GetString();
        role.Should().Be("model");
    }

    [Fact]
    public void BuildRequestBody_WithSystemMessage_ShouldMapToUserRole()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var messages = new[] { ChatMessage.CreateSystemMessage("System prompt") };
        var buildMethod = typeof(ChatClient).GetMethod("BuildRequestBody",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = buildMethod?.Invoke(chatClient, new object[] { messages, null, false });

        // Assert
        result.Should().NotBeNull();
        var json = result as string;
        var doc = JsonDocument.Parse(json!);
        var role = doc.RootElement.GetProperty("contents")[0].GetProperty("role").GetString();
        role.Should().Be("user");
    }

    [Fact]
    public void BuildRequestBody_WithOptions_ShouldIncludeGenerationConfig()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var messages = new[] { ChatMessage.CreateUserMessage("Hello") };
        var options = new ChatCompletionOptions
        {
            Temperature = 0.7,
            TopP = 0.9,
            TopK = 40,
            MaxTokens = 100
        };
        var buildMethod = typeof(ChatClient).GetMethod("BuildRequestBody",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = buildMethod?.Invoke(chatClient, new object[] { messages, options, false });

        // Assert
        result.Should().NotBeNull();
        var json = result as string;
        var doc = JsonDocument.Parse(json!);
        doc.RootElement.TryGetProperty("generationConfig", out var config).Should().BeTrue();
        config.GetProperty("temperature").GetDouble().Should().Be(0.7);
        config.GetProperty("topP").GetDouble().Should().Be(0.9);
        config.GetProperty("topK").GetInt32().Should().Be(40);
        config.GetProperty("maxOutputTokens").GetInt32().Should().Be(100);
    }

    [Fact]
    public void BuildRequestBody_WithStopSequences_ShouldIncludeInConfig()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var messages = new[] { ChatMessage.CreateUserMessage("Hello") };
        var options = new ChatCompletionOptions
        {
            StopSequences = new List<string> { "END", "STOP" }
        };
        var buildMethod = typeof(ChatClient).GetMethod("BuildRequestBody",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = buildMethod?.Invoke(chatClient, new object[] { messages, options, false });

        // Assert
        result.Should().NotBeNull();
        var json = result as string;
        var doc = JsonDocument.Parse(json!);
        var stopSequences = doc.RootElement.GetProperty("generationConfig").GetProperty("stopSequences");
        stopSequences.GetArrayLength().Should().Be(2);
    }

    [Fact]
    public void BuildRequestBody_WithMultipleMessages_ShouldCreateMultipleContents()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var messages = new[]
        {
            ChatMessage.CreateUserMessage("Hello"),
            ChatMessage.CreateAssistantMessage("Hi there!"),
            ChatMessage.CreateUserMessage("How are you?")
        };
        var buildMethod = typeof(ChatClient).GetMethod("BuildRequestBody",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = buildMethod?.Invoke(chatClient, new object[] { messages, null, false });

        // Assert
        result.Should().NotBeNull();
        var json = result as string;
        var doc = JsonDocument.Parse(json!);
        doc.RootElement.GetProperty("contents").GetArrayLength().Should().Be(3);
    }

    [Fact]
    public void ParseChatCompletion_WithValidResponse_ShouldParseCorrectly()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var responseJson = @"{
            ""candidates"": [{
                ""content"": {
                    ""parts"": [{""text"": ""Hello World""}]
                },
                ""finishReason"": ""STOP""
            }],
            ""usageMetadata"": {
                ""promptTokenCount"": 10,
                ""candidatesTokenCount"": 20
            }
        }";
        var parseMethod = typeof(ChatClient).GetMethod("ParseChatCompletion",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = parseMethod?.Invoke(chatClient, new object[] { responseJson });

        // Assert
        result.Should().NotBeNull();
        var completion = result as ChatCompletion;
        completion.Should().NotBeNull();
        completion!.Content.Should().Be("Hello World");
        completion.FinishReason.Should().Be("STOP");
    }

    [Fact]
    public void ParseChatCompletion_WithEmptyCandidates_ShouldReturnEmptyContent()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var responseJson = @"{""candidates"": []}";
        var parseMethod = typeof(ChatClient).GetMethod("ParseChatCompletion",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = parseMethod?.Invoke(chatClient, new object[] { responseJson });

        // Assert
        result.Should().NotBeNull();
        var completion = result as ChatCompletion;
        completion!.Content.Should().BeEmpty();
    }

    [Fact]
    public void ParseStreamingUpdate_WithValidData_ShouldParseCorrectly()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var data = @"{
            ""candidates"": [{
                ""content"": {
                    ""parts"": [{""text"": ""Partial""}]
                }
            }]
        }";
        var parseMethod = typeof(ChatClient).GetMethod("ParseStreamingUpdate",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = parseMethod?.Invoke(chatClient, new object[] { data });

        // Assert
        result.Should().NotBeNull();
        var update = result as StreamingChatCompletionUpdate;
        update!.ContentUpdate.Should().Be("Partial");
    }

    [Fact]
    public void ParseStreamingUpdate_WithInvalidData_ShouldReturnNull()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var data = @"invalid json";
        var parseMethod = typeof(ChatClient).GetMethod("ParseStreamingUpdate",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = parseMethod?.Invoke(chatClient, new object[] { data });

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseStreamingUpdate_WithEmptyCandidates_ShouldReturnNull()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var data = @"{""candidates"": []}";
        var parseMethod = typeof(ChatClient).GetMethod("ParseStreamingUpdate",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = parseMethod?.Invoke(chatClient, new object[] { data });

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void BuildRequestBody_WithMultipartContent_ShouldIncludeAllParts()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var contentParts = new List<ContentPart>
        {
            new TextContent("Part 1"),
            new TextContent("Part 2")
        };
        var messages = new[] { new ChatMessage("user", contentParts) };
        var buildMethod = typeof(ChatClient).GetMethod("BuildRequestBody",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = buildMethod?.Invoke(chatClient, new object[] { messages, null, false });

        // Assert
        result.Should().NotBeNull();
        var json = result as string;
        var doc = JsonDocument.Parse(json!);
        var parts = doc.RootElement.GetProperty("contents")[0].GetProperty("parts");
        parts.GetArrayLength().Should().Be(2);
        parts[0].GetProperty("text").GetString().Should().Be("Part 1");
        parts[1].GetProperty("text").GetString().Should().Be("Part 2");
    }

    [Fact]
    public void ParseChatCompletion_WithMultipleParts_ShouldConcatenateText()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var responseJson = @"{
            ""candidates"": [{
                ""content"": {
                    ""parts"": [
                        {""text"": ""Hello""},
                        {""text"": "" ""},
                        {""text"": ""World""}
                    ]
                }
            }]
        }";
        var parseMethod = typeof(ChatClient).GetMethod("ParseChatCompletion",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = parseMethod?.Invoke(chatClient, new object[] { responseJson });

        // Assert
        result.Should().NotBeNull();
        var completion = result as ChatCompletion;
        completion!.Content.Should().Be("Hello World");
    }

    [Theory]
    [InlineData("user", "user")]
    [InlineData("assistant", "model")]
    [InlineData("system", "user")]
    [InlineData("custom", "user")]
    public void BuildRequestBody_WithDifferentRoles_ShouldMapCorrectly(string inputRole, string expectedRole)
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var messages = new[] { new ChatMessage(inputRole, "Test message") };
        var buildMethod = typeof(ChatClient).GetMethod("BuildRequestBody",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = buildMethod?.Invoke(chatClient, new object[] { messages, null, false });

        // Assert
        result.Should().NotBeNull();
        var json = result as string;
        var doc = JsonDocument.Parse(json!);
        var role = doc.RootElement.GetProperty("contents")[0].GetProperty("role").GetString();
        role.Should().Be(expectedRole);
    }
}
