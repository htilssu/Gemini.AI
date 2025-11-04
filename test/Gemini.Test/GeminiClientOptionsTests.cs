using System.ClientModel;
using FluentAssertions;
using Gemini.Content;

namespace Gemini.Test;

public class GeminiClientOptionsTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithNullEndpoint()
    {
        // Act
        var options = new GeminiClientOptions();

        // Assert
        options.Endpoint.Should().BeNull();
    }

    [Fact]
    public void Endpoint_ShouldSetAndGetValue()
    {
        // Arrange
        var options = new GeminiClientOptions();
        var endpoint = new Uri("https://custom.endpoint.com");

        // Act
        options.Endpoint = endpoint;

        // Assert
        options.Endpoint.Should().Be(endpoint);
    }

    [Fact]
    public void Endpoint_ShouldAcceptNullValue()
    {
        // Arrange
        var options = new GeminiClientOptions
        {
            Endpoint = new Uri("https://test.com")
        };

        // Act
        options.Endpoint = null;

        // Assert
        options.Endpoint.Should().BeNull();
    }

    [Theory]
    [InlineData("https://api.example.com")]
    [InlineData("https://test.generativelanguage.googleapis.com")]
    [InlineData("http://localhost:8080")]
    public void Endpoint_ShouldAcceptVariousUris(string uriString)
    {
        // Arrange
        var options = new GeminiClientOptions();
        var uri = new Uri(uriString);

        // Act
        options.Endpoint = uri;

        // Assert
        options.Endpoint.Should().Be(uri);
        options.Endpoint.ToString().Should().StartWith(uriString);
    }

    [Fact]
    public void GeminiClientOptions_ShouldInheritFromClientPipelineOptions()
    {
        // Arrange & Act
        var options = new GeminiClientOptions();

        // Assert
        options.Should().BeAssignableTo<System.ClientModel.Primitives.ClientPipelineOptions>();
    }
}
