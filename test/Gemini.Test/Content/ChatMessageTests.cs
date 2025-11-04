using FluentAssertions;
using Gemini.Content;

namespace Gemini.Test.Content;

public class ChatMessageTests
{
    [Fact]
    public void Constructor_WithRoleAndContent_ShouldCreateMessage()
    {
        // Arrange
        var role = "user";
        var content = "Hello, world!";

        // Act
        var message = new ChatMessage(role, content);

        // Assert
        message.Role.Should().Be(role);
        message.Contents.Should().ContainSingle();
        message.Contents[0].Should().BeOfType<TextContent>();
        ((TextContent)message.Contents[0]).Text.Should().Be(content);
    }

    [Fact]
    public void Constructor_WithRoleAndContentParts_ShouldCreateMessage()
    {
        // Arrange
        var role = "assistant";
        var contents = new List<ContentPart>
        {
            new TextContent("Part 1"),
            new TextContent("Part 2")
        };

        // Act
        var message = new ChatMessage(role, contents);

        // Assert
        message.Role.Should().Be(role);
        message.Contents.Should().HaveCount(2);
        ((TextContent)message.Contents[0]).Text.Should().Be("Part 1");
        ((TextContent)message.Contents[1]).Text.Should().Be("Part 2");
    }

    [Fact]
    public void CreateUserMessage_ShouldCreateUserMessage()
    {
        // Arrange
        var content = "User message";

        // Act
        var message = ChatMessage.CreateUserMessage(content);

        // Assert
        message.Role.Should().Be("user");
        message.Contents.Should().ContainSingle();
        ((TextContent)message.Contents[0]).Text.Should().Be(content);
    }

    [Fact]
    public void CreateAssistantMessage_ShouldCreateAssistantMessage()
    {
        // Arrange
        var content = "Assistant message";

        // Act
        var message = ChatMessage.CreateAssistantMessage(content);

        // Assert
        message.Role.Should().Be("assistant");
        message.Contents.Should().ContainSingle();
        ((TextContent)message.Contents[0]).Text.Should().Be(content);
    }

    [Fact]
    public void CreateSystemMessage_ShouldCreateSystemMessage()
    {
        // Arrange
        var content = "System message";

        // Act
        var message = ChatMessage.CreateSystemMessage(content);

        // Assert
        message.Role.Should().Be("system");
        message.Contents.Should().ContainSingle();
        ((TextContent)message.Contents[0]).Text.Should().Be(content);
    }

    [Fact]
    public void Role_ShouldBeSettable()
    {
        // Arrange
        var message = new ChatMessage("user", "content");

        // Act
        message.Role = "assistant";

        // Assert
        message.Role.Should().Be("assistant");
    }

    [Fact]
    public void Contents_ShouldBeSettable()
    {
        // Arrange
        var message = new ChatMessage("user", "content");
        var newContents = new List<ContentPart> { new TextContent("New content") };

        // Act
        message.Contents = newContents;

        // Assert
        message.Contents.Should().BeEquivalentTo(newContents);
    }

    [Fact]
    public void Constructor_WithEmptyContent_ShouldCreateEmptyTextContent()
    {
        // Arrange & Act
        var message = new ChatMessage("user", string.Empty);

        // Assert
        message.Contents.Should().ContainSingle();
        ((TextContent)message.Contents[0]).Text.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithMultipleContentParts_ShouldPreserveOrder()
    {
        // Arrange
        var contents = new List<ContentPart>
        {
            new TextContent("First"),
            new TextContent("Second"),
            new TextContent("Third")
        };

        // Act
        var message = new ChatMessage("user", contents);

        // Assert
        message.Contents.Should().HaveCount(3);
        ((TextContent)message.Contents[0]).Text.Should().Be("First");
        ((TextContent)message.Contents[1]).Text.Should().Be("Second");
        ((TextContent)message.Contents[2]).Text.Should().Be("Third");
    }

    [Theory]
    [InlineData("user")]
    [InlineData("assistant")]
    [InlineData("system")]
    [InlineData("custom_role")]
    public void Constructor_ShouldAcceptVariousRoles(string role)
    {
        // Act
        var message = new ChatMessage(role, "content");

        // Assert
        message.Role.Should().Be(role);
    }
}
