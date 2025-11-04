using System.Reflection;
using FluentAssertions;
using Gemini.Agent.AI;
using Gemini.Content;
using Microsoft.Extensions.AI;

namespace Gemini.Test;

public class GeminiChatClientIntegrationTests
{
    private const string TestApiKey = "test-api-key";
    private const string TestModel = "gemini-pro";

    [Fact]
    public void ConvertToGeminiMessages_ShouldConvertMultipleMessages()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);
        var messages = new[]
        {
            new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, "Hello"),
            new Microsoft.Extensions.AI.ChatMessage(ChatRole.Assistant, "Hi there"),
            new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, "How are you?")
        };
        
        var method = typeof(GeminiChatClient).GetMethod("ConvertToGeminiMessages",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(geminiChatClient, new object[] { messages });

        // Assert
        result.Should().NotBeNull();
        var geminiMessages = result as IEnumerable<Gemini.Content.ChatMessage>;
        geminiMessages.Should().NotBeNull();
        geminiMessages.Should().HaveCount(3);
    }

    [Fact]
    public void ConvertToGeminiOptions_WithNullOptions_ShouldReturnNull()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);
        var method = typeof(GeminiChatClient).GetMethod("ConvertToGeminiOptions",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(geminiChatClient, new object?[] { null });

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ConvertToGeminiOptions_WithTemperature_ShouldMapTemperature()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);
        var options = new ChatOptions { Temperature = 0.7f };
        var method = typeof(GeminiChatClient).GetMethod("ConvertToGeminiOptions",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(geminiChatClient, new object[] { options });

        // Assert
        result.Should().NotBeNull();
        var geminiOptions = result as ChatCompletionOptions;
        geminiOptions!.Temperature.Should().BeApproximately(0.7, 0.0001);
    }

    [Fact]
    public void ConvertToGeminiOptions_WithTopP_ShouldMapTopP()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);
        var options = new ChatOptions { TopP = 0.9f };
        var method = typeof(GeminiChatClient).GetMethod("ConvertToGeminiOptions",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(geminiChatClient, new object[] { options });

        // Assert
        result.Should().NotBeNull();
        var geminiOptions = result as ChatCompletionOptions;
        geminiOptions!.TopP.Should().BeApproximately(0.9, 0.0001);
    }

    [Fact]
    public void ConvertToGeminiOptions_WithMaxOutputTokens_ShouldMapToMaxTokens()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);
        var options = new ChatOptions { MaxOutputTokens = 1024 };
        var method = typeof(GeminiChatClient).GetMethod("ConvertToGeminiOptions",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(geminiChatClient, new object[] { options });

        // Assert
        result.Should().NotBeNull();
        var geminiOptions = result as ChatCompletionOptions;
        geminiOptions!.MaxTokens.Should().Be(1024);
    }

    [Fact]
    public void ConvertToGeminiOptions_WithStopSequences_ShouldMapStopSequences()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);
        var options = new ChatOptions { StopSequences = new[] { "END", "STOP" } };
        var method = typeof(GeminiChatClient).GetMethod("ConvertToGeminiOptions",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(geminiChatClient, new object[] { options });

        // Assert
        result.Should().NotBeNull();
        var geminiOptions = result as ChatCompletionOptions;
        geminiOptions!.StopSequences.Should().HaveCount(2);
        geminiOptions.StopSequences.Should().Contain("END");
        geminiOptions.StopSequences.Should().Contain("STOP");
    }

    [Fact]
    public void ConvertToGeminiOptions_WithAllOptions_ShouldMapAllProperties()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);
        var options = new ChatOptions
        {
            Temperature = 0.8f,
            TopP = 0.95f,
            MaxOutputTokens = 2048,
            StopSequences = new[] { "END" }
        };
        var method = typeof(GeminiChatClient).GetMethod("ConvertToGeminiOptions",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(geminiChatClient, new object[] { options });

        // Assert
        result.Should().NotBeNull();
        var geminiOptions = result as ChatCompletionOptions;
        geminiOptions!.Temperature.Should().BeApproximately(0.8, 0.0001);
        geminiOptions.TopP.Should().BeApproximately(0.95, 0.0001);
        geminiOptions.MaxTokens.Should().Be(2048);
        geminiOptions.StopSequences.Should().ContainSingle();
    }

    [Fact]
    public void ConvertToGeminiMessages_WithEmptyList_ShouldReturnEmptyList()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);
        var messages = Array.Empty<Microsoft.Extensions.AI.ChatMessage>();
        var method = typeof(GeminiChatClient).GetMethod("ConvertToGeminiMessages",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(geminiChatClient, new object[] { messages });

        // Assert
        result.Should().NotBeNull();
        var geminiMessages = result as IEnumerable<Gemini.Content.ChatMessage>;
        geminiMessages.Should().BeEmpty();
    }

    [Fact]
    public void ConvertToGeminiOptions_WithNullStopSequences_ShouldNotIncludeStopSequences()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);
        var options = new ChatOptions { Temperature = 0.7f };
        var method = typeof(GeminiChatClient).GetMethod("ConvertToGeminiOptions",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(geminiChatClient, new object[] { options });

        // Assert
        result.Should().NotBeNull();
        var geminiOptions = result as ChatCompletionOptions;
        geminiOptions!.StopSequences.Should().BeNull();
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(0.5f)]
    [InlineData(1.0f)]
    [InlineData(2.0f)]
    public void ConvertToGeminiOptions_WithVariousTemperatures_ShouldMapCorrectly(float temperature)
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);
        var options = new ChatOptions { Temperature = temperature };
        var method = typeof(GeminiChatClient).GetMethod("ConvertToGeminiOptions",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(geminiChatClient, new object[] { options });

        // Assert
        result.Should().NotBeNull();
        var geminiOptions = result as ChatCompletionOptions;
        geminiOptions!.Temperature.Should().BeApproximately(temperature, 0.0001);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1024)]
    [InlineData(4096)]
    public void ConvertToGeminiOptions_WithVariousMaxTokens_ShouldMapCorrectly(int maxTokens)
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);
        var options = new ChatOptions { MaxOutputTokens = maxTokens };
        var method = typeof(GeminiChatClient).GetMethod("ConvertToGeminiOptions",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(geminiChatClient, new object[] { options });

        // Assert
        result.Should().NotBeNull();
        var geminiOptions = result as ChatCompletionOptions;
        geminiOptions!.MaxTokens.Should().Be(maxTokens);
    }
}
