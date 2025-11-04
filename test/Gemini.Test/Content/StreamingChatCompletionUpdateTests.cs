using FluentAssertions;
using Gemini.Content;

namespace Gemini.Test.Content;

public class StreamingChatCompletionUpdateTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        var contentUpdate = "Streaming content";
        var finishReason = "stop";

        // Act
        var update = new StreamingChatCompletionUpdate(contentUpdate, finishReason);

        // Assert
        update.ContentUpdate.Should().Be(contentUpdate);
        update.FinishReason.Should().Be(finishReason);
    }

    [Fact]
    public void Constructor_WithNullFinishReason_ShouldHandleNull()
    {
        // Arrange
        var contentUpdate = "Content";

        // Act
        var update = new StreamingChatCompletionUpdate(contentUpdate, null);

        // Assert
        update.ContentUpdate.Should().Be(contentUpdate);
        update.FinishReason.Should().BeNull();
    }

    [Fact]
    public void ToString_ShouldReturnContentUpdate()
    {
        // Arrange
        var contentUpdate = "Test update";
        var update = new StreamingChatCompletionUpdate(contentUpdate, "stop");

        // Act
        var result = update.ToString();

        // Assert
        result.Should().Be(contentUpdate);
    }

    [Fact]
    public void ToString_WithEmptyContent_ShouldReturnEmptyString()
    {
        // Arrange
        var update = new StreamingChatCompletionUpdate(string.Empty, null);

        // Act
        var result = update.ToString();

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
        var update = new StreamingChatCompletionUpdate("content", finishReason);

        // Assert
        update.FinishReason.Should().Be(finishReason);
    }

    [Fact]
    public void Constructor_WithPartialContent_ShouldStorePartial()
    {
        // Arrange
        var partial = "Part";

        // Act
        var update = new StreamingChatCompletionUpdate(partial, null);

        // Assert
        update.ContentUpdate.Should().Be(partial);
    }

    [Theory]
    [InlineData("Single word")]
    [InlineData("Multiple words here")]
    [InlineData("")]
    [InlineData("Special!@#$%")]
    public void ContentUpdate_ShouldAcceptVariousStrings(string content)
    {
        // Act
        var update = new StreamingChatCompletionUpdate(content, null);

        // Assert
        update.ContentUpdate.Should().Be(content);
    }

    [Fact]
    public void Constructor_WithUnicodeContent_ShouldPreserveUnicode()
    {
        // Arrange
        var unicode = "Unicode: 你好 🌍";

        // Act
        var update = new StreamingChatCompletionUpdate(unicode, null);

        // Assert
        update.ContentUpdate.Should().Be(unicode);
    }
}
