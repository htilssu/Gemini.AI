using FluentAssertions;
using Gemini.Content;

namespace Gemini.Test.Content;

public class ContentPartTests
{
    [Fact]
    public void ContentPart_ShouldBeAbstract()
    {
        // Arrange
        var type = typeof(ContentPart);

        // Assert
        type.IsAbstract.Should().BeTrue();
    }

    [Fact]
    public void TextContent_ShouldDeriveFromContentPart()
    {
        // Arrange
        var textContent = new TextContent("test");

        // Assert
        textContent.Should().BeAssignableTo<ContentPart>();
    }

    [Fact]
    public void ContentPart_ShouldBeAssignableToTextContent()
    {
        // Arrange
        ContentPart contentPart = new TextContent("test");

        // Assert
        contentPart.Should().NotBeNull();
        contentPart.Should().BeOfType<TextContent>();
    }
}
