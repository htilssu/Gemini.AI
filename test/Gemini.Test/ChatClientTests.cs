using System.ClientModel;
using FluentAssertions;
using Gemini.Content;

namespace Gemini.Test;

public class ChatClientTests
{
    private const string TestApiKey = "test-api-key-12345";
    private const string TestModel = "gemini-pro";

    [Fact]
    public void Constructor_WithModelAndStringApiKey_ShouldCreateClient()
    {
        // Act
        var client = new ChatClient(TestModel, TestApiKey);

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithModelAndApiKeyCredential_ShouldCreateClient()
    {
        // Arrange
        var apiKey = new ApiKeyCredential(TestApiKey);

        // Act
        var client = new ChatClient(TestModel, apiKey);

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithAllParameters_ShouldCreateClient()
    {
        // Arrange
        var apiKey = new ApiKeyCredential(TestApiKey);
        var options = new GeminiClientOptions
        {
            Endpoint = new Uri("https://custom.endpoint.com")
        };

        // Act
        var client = new ChatClient(TestModel, apiKey, options);

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullModel_ShouldThrowArgumentNullException()
    {
        // Arrange
        var apiKey = new ApiKeyCredential(TestApiKey);

        // Act
        var act = () => new ChatClient(null!, apiKey);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("model");
    }

    [Fact]
    public void Constructor_WithNullApiKey_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new ChatClient(TestModel, (ApiKeyCredential)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("apiKey");
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Arrange
        var apiKey = new ApiKeyCredential(TestApiKey);

        // Act
        var act = () => new ChatClient(TestModel, apiKey, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("options");
    }

    [Fact]
    public void Constructor_WithNullEndpointInOptions_ShouldSetDefaultEndpoint()
    {
        // Arrange
        var apiKey = new ApiKeyCredential(TestApiKey);
        var options = new GeminiClientOptions();

        // Act
        var client = new ChatClient(TestModel, apiKey, options);

        // Assert
        client.Should().NotBeNull();
        options.Endpoint.Should().NotBeNull();
        options.Endpoint.ToString().Should().Be("https://generativelanguage.googleapis.com/");
    }

    [Fact]
    public void Constructor_WithCustomEndpoint_ShouldUseCustomEndpoint()
    {
        // Arrange
        var apiKey = new ApiKeyCredential(TestApiKey);
        var customEndpoint = new Uri("https://custom.api.com");
        var options = new GeminiClientOptions
        {
            Endpoint = customEndpoint
        };

        // Act
        var client = new ChatClient(TestModel, apiKey, options);

        // Assert
        client.Should().NotBeNull();
        options.Endpoint.Should().Be(customEndpoint);
    }

    [Theory]
    [InlineData("gemini-pro")]
    [InlineData("gemini-1.5-flash")]
    [InlineData("gemini-1.5-pro")]
    [InlineData("custom-model")]
    public void Constructor_ShouldAcceptVariousModelNames(string model)
    {
        // Arrange
        var apiKey = new ApiKeyCredential(TestApiKey);

        // Act
        var client = new ChatClient(model, apiKey);

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void CompleteChatAsync_WithNullMessages_ShouldThrowException()
    {
        // Arrange
        var client = new ChatClient(TestModel, TestApiKey);

        // Act
        Func<Task> act = async () => await client.CompleteChatAsync(null!);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CompleteChatStreamingAsync_WithNullMessages_ShouldThrowException()
    {
        // Arrange
        var client = new ChatClient(TestModel, TestApiKey);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async () =>
        {
            await foreach (var update in client.CompleteChatStreamingAsync(null!))
            {
                // Should not reach here
            }
        });
    }
}
