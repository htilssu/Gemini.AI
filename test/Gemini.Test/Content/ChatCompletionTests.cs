using System.Text.Json;
using FluentAssertions;
using Gemini.Content;

namespace Gemini.Test.Content;

public class ChatCompletionTests
{
    [Fact]
    public void Constructor_ShouldInitializeAllProperties()
    {
        // Arrange
        var content = "Test content";
        var finishReason = "stop";
        var usageJson = JsonDocument.Parse("{\"promptTokens\": 10, \"completionTokens\": 20}").RootElement;
        var rawJson = JsonDocument.Parse("{\"test\": \"data\"}").RootElement;

        // Act
        var chatCompletion = new ChatCompletion(content, finishReason, usageJson, rawJson);

        // Assert
        chatCompletion.Content.Should().Be(content);
        chatCompletion.FinishReason.Should().Be(finishReason);
        chatCompletion.UsageMetadata.Should().NotBeNull();
        chatCompletion.RawResponse.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullValues_ShouldHandleNulls()
    {
        // Arrange & Act
        var chatCompletion = new ChatCompletion("content", null, null, null);

        // Assert
        chatCompletion.Content.Should().Be("content");
        chatCompletion.FinishReason.Should().BeNull();
        chatCompletion.UsageMetadata.Should().BeNull();
        chatCompletion.RawResponse.Should().BeNull();
    }

    [Fact]
    public void ToString_ShouldReturnContent()
    {
        // Arrange
        var content = "Test response content";
        var chatCompletion = new ChatCompletion(content, "stop", null, null);

        // Act
        var result = chatCompletion.ToString();

        // Assert
        result.Should().Be(content);
    }

    [Fact]
    public void ToString_WithEmptyContent_ShouldReturnEmptyString()
    {
        // Arrange
        var chatCompletion = new ChatCompletion(string.Empty, null, null, null);

        // Act
        var result = chatCompletion.ToString();

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData("stop")]
    [InlineData("length")]
    [InlineData("content_filter")]
    [InlineData(null)]
    public void FinishReason_ShouldAcceptVariousValues(string? finishReason)
    {
        // Arrange & Act
        var chatCompletion = new ChatCompletion("content", finishReason, null, null);

        // Assert
        chatCompletion.FinishReason.Should().Be(finishReason);
    }
}
