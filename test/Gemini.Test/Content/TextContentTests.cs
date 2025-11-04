using FluentAssertions;
using Gemini.Content;

namespace Gemini.Test.Content;

public class TextContentTests
{
    [Fact]
    public void Constructor_ShouldSetText()
    {
        // Arrange
        var text = "Sample text";

        // Act
        var textContent = new TextContent(text);

        // Assert
        textContent.Text.Should().Be(text);
    }

    [Fact]
    public void TextContent_ShouldInheritFromContentPart()
    {
        // Arrange & Act
        var textContent = new TextContent("text");

        // Assert
        textContent.Should().BeAssignableTo<ContentPart>();
    }

    [Fact]
    public void Text_ShouldBeSettable()
    {
        // Arrange
        var textContent = new TextContent("initial");

        // Act
        textContent.Text = "updated";

        // Assert
        textContent.Text.Should().Be("updated");
    }

    [Fact]
    public void Constructor_WithEmptyString_ShouldSetEmptyText()
    {
        // Arrange & Act
        var textContent = new TextContent(string.Empty);

        // Assert
        textContent.Text.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithWhitespace_ShouldPreserveWhitespace()
    {
        // Arrange
        var text = "  whitespace  ";

        // Act
        var textContent = new TextContent(text);

        // Assert
        textContent.Text.Should().Be(text);
    }

    [Fact]
    public void Constructor_WithMultilineText_ShouldPreserveNewlines()
    {
        // Arrange
        var text = "Line 1\nLine 2\nLine 3";

        // Act
        var textContent = new TextContent(text);

        // Assert
        textContent.Text.Should().Be(text);
    }

    [Fact]
    public void Constructor_WithSpecialCharacters_ShouldPreserveCharacters()
    {
        // Arrange
        var text = "Special chars: !@#$%^&*()_+-=[]{}|;':\",./<>?";

        // Act
        var textContent = new TextContent(text);

        // Assert
        textContent.Text.Should().Be(text);
    }

    [Fact]
    public void Constructor_WithUnicodeCharacters_ShouldPreserveUnicode()
    {
        // Arrange
        var text = "Unicode: 你好 🌍 مرحبا";

        // Act
        var textContent = new TextContent(text);

        // Assert
        textContent.Text.Should().Be(text);
    }

    [Theory]
    [InlineData("Simple text")]
    [InlineData("")]
    [InlineData("123456")]
    [InlineData("Mixed 123 ABC !@#")]
    public void Constructor_WithVariousInputs_ShouldSetText(string text)
    {
        // Act
        var textContent = new TextContent(text);

        // Assert
        textContent.Text.Should().Be(text);
    }
}
