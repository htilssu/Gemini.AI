using FluentAssertions;
using Gemini.Content.Agent.AI.Extensions;
using Microsoft.Extensions.AI;

namespace Gemini.Test.Extensions;

public class ChatMessageExtensionTests
{
    [Fact]
    public void ToGeminiChatMessage_WithUserMessage_ShouldConvertToGeminiMessage()
    {
        // Arrange
        var aiMessage = new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, "Hello");

        // Act
        var result = aiMessage.ToGeminiChatMessage();

        // Assert
        result.Should().NotBeNull();
        result.Role.Should().Be("user");
        result.Contents.Should().ContainSingle();
    }

    [Fact]
    public void ToGeminiChatMessage_WithAssistantMessage_ShouldConvertWithUserRole()
    {
        // Arrange
        var aiMessage = new Microsoft.Extensions.AI.ChatMessage(ChatRole.Assistant, "Response");

        // Act
        var result = aiMessage.ToGeminiChatMessage();

        // Assert
        result.Should().NotBeNull();
        result.Role.Should().Be("user");
        result.Contents.Should().ContainSingle();
    }

    [Fact]
    public void ToGeminiChatMessage_WithSystemMessage_ShouldConvertWithUserRole()
    {
        // Arrange
        var aiMessage = new Microsoft.Extensions.AI.ChatMessage(ChatRole.System, "System prompt");

        // Act
        var result = aiMessage.ToGeminiChatMessage();

        // Assert
        result.Should().NotBeNull();
        result.Role.Should().Be("user");
        result.Contents.Should().ContainSingle();
    }

    [Fact]
    public void ToGeminiChatMessage_WithMultipleContents_ShouldConvertAll()
    {
        // Arrange
        var aiMessage = new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, 
            new List<AIContent> { new Microsoft.Extensions.AI.TextContent("Part 1"), new Microsoft.Extensions.AI.TextContent("Part 2") });

        // Act
        var result = aiMessage.ToGeminiChatMessage();

        // Assert
        result.Should().NotBeNull();
        result.Contents.Should().HaveCount(2);
    }

    [Fact]
    public void ToGeminiChatMessage_WithEmptyContent_ShouldCreateEmptyContentsList()
    {
        // Arrange
        var aiMessage = new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, new List<AIContent>());

        // Act
        var result = aiMessage.ToGeminiChatMessage();

        // Assert
        result.Should().NotBeNull();
        result.Contents.Should().BeEmpty();
    }

    [Theory]
    [InlineData("Simple message")]
    [InlineData("")]
    [InlineData("Multi\nline\ntext")]
    [InlineData("Unicode 你好 🌍")]
    public void ToGeminiChatMessage_WithVariousTexts_ShouldConvertCorrectly(string text)
    {
        // Arrange
        var aiMessage = new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, text);

        // Act
        var result = aiMessage.ToGeminiChatMessage();

        // Assert
        result.Should().NotBeNull();
        result.Contents.Should().ContainSingle();
    }
}
