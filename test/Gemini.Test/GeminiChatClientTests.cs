using FluentAssertions;
using Gemini.Agent.AI;
using Gemini.Content;
using Microsoft.Extensions.AI;
using Moq;

namespace Gemini.Test;

public class GeminiChatClientTests
{
    private const string TestApiKey = "test-api-key";
    private const string TestModel = "gemini-pro";

    [Fact]
    public void Constructor_WithChatClient_ShouldCreateGeminiChatClient()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);

        // Act
        var geminiChatClient = new GeminiChatClient(chatClient);

        // Assert
        geminiChatClient.Should().NotBeNull();
        geminiChatClient.Metadata.Should().NotBeNull();
        geminiChatClient.Metadata.ProviderName.Should().Be("Gemini");
    }

    [Fact]
    public void Constructor_WithModelId_ShouldCreateMetadata()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var modelId = "custom-model-id";

        // Act
        var geminiChatClient = new GeminiChatClient(chatClient, modelId);

        // Assert
        geminiChatClient.Metadata.Should().NotBeNull();
        geminiChatClient.Metadata.ProviderName.Should().Be("Gemini");
    }

    [Fact]
    public void Constructor_WithNullChatClient_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new GeminiChatClient(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("chatClient");
    }

    [Fact]
    public void Metadata_ShouldHaveProviderNameGemini()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);

        // Assert
        geminiChatClient.Metadata.ProviderName.Should().Be("Gemini");
    }

    [Fact]
    public void Metadata_WithoutModelId_ShouldNotBeNull()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);

        // Assert
        geminiChatClient.Metadata.Should().NotBeNull();
    }

    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);

        // Act
        var act = () => geminiChatClient.Dispose();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void GetService_WithChatClientType_ShouldReturnChatClient()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);

        // Act
        var result = geminiChatClient.GetService(typeof(ChatClient));

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(chatClient);
    }

    [Fact]
    public void GetService_WithOtherType_ShouldReturnNull()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);

        // Act
        var result = geminiChatClient.GetService(typeof(string));

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetServiceGeneric_WithChatClient_ShouldReturnChatClient()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);

        // Act
        var result = geminiChatClient.GetService<ChatClient>();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(chatClient);
    }

    [Fact]
    public void GetServiceGeneric_WithOtherType_ShouldReturnNull()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);

        // Act
        var result = geminiChatClient.GetService<string>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GeminiChatClient_ShouldImplementIChatClient()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);

        // Act
        var geminiChatClient = new GeminiChatClient(chatClient);

        // Assert
        geminiChatClient.Should().BeAssignableTo<IChatClient>();
    }

    [Theory]
    [InlineData("model-1")]
    [InlineData("model-2")]
    [InlineData(null)]
    public void Constructor_WithVariousModelIds_ShouldSetCorrectly(string? modelId)
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);

        // Act
        var geminiChatClient = new GeminiChatClient(chatClient, modelId);

        // Assert
        geminiChatClient.Metadata.Should().NotBeNull();
    }

    [Fact]
    public void GetService_WithServiceKey_ShouldReturnChatClient()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);

        // Act
        var result = geminiChatClient.GetService(typeof(ChatClient), "someKey");

        // Assert
        result.Should().BeSameAs(chatClient);
    }

    [Fact]
    public void GetServiceGeneric_WithServiceKey_ShouldReturnChatClient()
    {
        // Arrange
        var chatClient = new ChatClient(TestModel, TestApiKey);
        var geminiChatClient = new GeminiChatClient(chatClient);

        // Act
        var result = geminiChatClient.GetService<ChatClient>("someKey");

        // Assert
        result.Should().BeSameAs(chatClient);
    }
}
