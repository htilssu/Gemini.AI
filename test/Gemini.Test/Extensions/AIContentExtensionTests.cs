using FluentAssertions;
using Gemini.Content;
using Gemini.Content.Agent.AI.Extensions;
using Microsoft.Extensions.AI;
using AITextContent = Microsoft.Extensions.AI.TextContent;
using GeminiTextContent = Gemini.Content.TextContent;

namespace Gemini.Test.Extensions;

public class AIContentExtensionTests
{
    [Fact]
    public void ToGeminiContent_WithTextContent_ShouldReturnTextContent()
    {
        // Arrange
        var aiContent = new AITextContent("Test text");

        // Act
        var result = aiContent.ToGeminiContent();

        // Assert
        result.Should().BeOfType<GeminiTextContent>();
        ((GeminiTextContent)result).Text.Should().Be("Test text");
    }

    [Fact]
    public void ToGeminiContent_WithEmptyText_ShouldReturnEmptyTextContent()
    {
        // Arrange
        var aiContent = new AITextContent(string.Empty);

        // Act
        var result = aiContent.ToGeminiContent();

        // Assert
        result.Should().BeOfType<GeminiTextContent>();
        ((GeminiTextContent)result).Text.Should().BeEmpty();
    }

    [Fact]
    public void ToGeminiContent_WithNullToString_ShouldHandleNull()
    {
        // Arrange
        var aiContent = new AITextContent("test");

        // Act
        var result = aiContent.ToGeminiContent();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<GeminiTextContent>();
    }

    [Theory]
    [InlineData("Simple text")]
    [InlineData("Multi\nline\ntext")]
    [InlineData("Special chars !@#$%")]
    [InlineData("Unicode 你好 🌍")]
    public void ToGeminiContent_WithVariousTexts_ShouldConvertCorrectly(string text)
    {
        // Arrange
        var aiContent = new AITextContent(text);

        // Act
        var result = aiContent.ToGeminiContent();

        // Assert
        result.Should().BeOfType<GeminiTextContent>();
        ((GeminiTextContent)result).Text.Should().Be(text);
    }
}
