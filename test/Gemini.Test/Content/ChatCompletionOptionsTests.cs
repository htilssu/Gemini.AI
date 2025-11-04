using FluentAssertions;
using Gemini.Content;

namespace Gemini.Test.Content;

public class ChatCompletionOptionsTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var options = new ChatCompletionOptions();

        // Assert
        options.Temperature.Should().BeNull();
        options.TopP.Should().BeNull();
        options.TopK.Should().BeNull();
        options.MaxTokens.Should().BeNull();
        options.StopSequences.Should().BeNull();
    }

    [Fact]
    public void Temperature_ShouldSetAndGetValue()
    {
        // Arrange
        var options = new ChatCompletionOptions();
        var temperature = 0.7;

        // Act
        options.Temperature = temperature;

        // Assert
        options.Temperature.Should().Be(temperature);
    }

    [Fact]
    public void TopP_ShouldSetAndGetValue()
    {
        // Arrange
        var options = new ChatCompletionOptions();
        var topP = 0.9;

        // Act
        options.TopP = topP;

        // Assert
        options.TopP.Should().Be(topP);
    }

    [Fact]
    public void TopK_ShouldSetAndGetValue()
    {
        // Arrange
        var options = new ChatCompletionOptions();
        var topK = 40;

        // Act
        options.TopK = topK;

        // Assert
        options.TopK.Should().Be(topK);
    }

    [Fact]
    public void MaxTokens_ShouldSetAndGetValue()
    {
        // Arrange
        var options = new ChatCompletionOptions();
        var maxTokens = 1024;

        // Act
        options.MaxTokens = maxTokens;

        // Assert
        options.MaxTokens.Should().Be(maxTokens);
    }

    [Fact]
    public void StopSequences_ShouldSetAndGetValue()
    {
        // Arrange
        var options = new ChatCompletionOptions();
        var stopSequences = new List<string> { "END", "STOP" };

        // Act
        options.StopSequences = stopSequences;

        // Assert
        options.StopSequences.Should().BeEquivalentTo(stopSequences);
    }

    [Fact]
    public void AllProperties_ShouldSetSimultaneously()
    {
        // Arrange
        var options = new ChatCompletionOptions
        {
            Temperature = 0.8,
            TopP = 0.95,
            TopK = 50,
            MaxTokens = 2048,
            StopSequences = new List<string> { "END" }
        };

        // Assert
        options.Temperature.Should().Be(0.8);
        options.TopP.Should().Be(0.95);
        options.TopK.Should().Be(50);
        options.MaxTokens.Should().Be(2048);
        options.StopSequences.Should().ContainSingle().Which.Should().Be("END");
    }

    [Fact]
    public void StopSequences_ShouldHandleEmptyList()
    {
        // Arrange
        var options = new ChatCompletionOptions
        {
            StopSequences = new List<string>()
        };

        // Assert
        options.StopSequences.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    [InlineData(2.0)]
    public void Temperature_ShouldAcceptVariousValues(double temperature)
    {
        // Arrange
        var options = new ChatCompletionOptions();

        // Act
        options.Temperature = temperature;

        // Assert
        options.Temperature.Should().Be(temperature);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(8192)]
    public void MaxTokens_ShouldAcceptVariousValues(int maxTokens)
    {
        // Arrange
        var options = new ChatCompletionOptions();

        // Act
        options.MaxTokens = maxTokens;

        // Assert
        options.MaxTokens.Should().Be(maxTokens);
    }
}
