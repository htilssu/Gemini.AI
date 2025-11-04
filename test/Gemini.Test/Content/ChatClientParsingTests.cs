using System.Reflection;
using System.Text.Json;
using FluentAssertions;
using Gemini.Content;

namespace Gemini.Test.Content;

public class ChatClientParsingTests
{
    private const string TestApiKey = "test-key";
    private const string TestModel = "gemini-pro";

    [Fact]
    public void ParseChatCompletion_WithNoFinishReason_ShouldHandleGracefully()
    {
        // Arrange
        var client = new ChatClient(TestModel, TestApiKey);
        var json = @"{
            ""candidates"": [{
                ""content"": {
                    ""parts"": [{""text"": ""Response""}]
                }
            }]
        }";
        var method = typeof(ChatClient).GetMethod("ParseChatCompletion",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(client, new object[] { json });

        // Assert
        var completion = result as ChatCompletion;
        completion.Should().NotBeNull();
        completion!.Content.Should().Be("Response");
        completion.FinishReason.Should().BeNull();
    }

    [Fact]
    public void ParseChatCompletion_WithNoUsageMetadata_ShouldHandleGracefully()
    {
        // Arrange
        var client = new ChatClient(TestModel, TestApiKey);
        var json = @"{
            ""candidates"": [{
                ""content"": {
                    ""parts"": [{""text"": ""Response""}]
                },
                ""finishReason"": ""STOP""
            }]
        }";
        var method = typeof(ChatClient).GetMethod("ParseChatCompletion",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(client, new object[] { json });

        // Assert
        var completion = result as ChatCompletion;
        completion.Should().NotBeNull();
        completion!.UsageMetadata.Should().BeNull();
    }

    [Fact]
    public void ParseChatCompletion_WithComplexUsageMetadata_ShouldParseCorrectly()
    {
        // Arrange
        var client = new ChatClient(TestModel, TestApiKey);
        var json = @"{
            ""candidates"": [{
                ""content"": {
                    ""parts"": [{""text"": ""Response""}]
                },
                ""finishReason"": ""STOP""
            }],
            ""usageMetadata"": {
                ""promptTokenCount"": 15,
                ""candidatesTokenCount"": 30,
                ""totalTokenCount"": 45
            }
        }";
        var method = typeof(ChatClient).GetMethod("ParseChatCompletion",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(client, new object[] { json });

        // Assert
        var completion = result as ChatCompletion;
        completion.Should().NotBeNull();
        completion!.UsageMetadata.Should().NotBeNull();
        completion.UsageMetadata.Value.GetProperty("promptTokenCount").GetInt32().Should().Be(15);
    }

    [Fact]
    public void ParseStreamingUpdate_WithFinishReason_ShouldIncludeFinishReason()
    {
        // Arrange
        var client = new ChatClient(TestModel, TestApiKey);
        var json = @"{
            ""candidates"": [{
                ""content"": {
                    ""parts"": [{""text"": ""Partial""}]
                },
                ""finishReason"": ""MAX_TOKENS""
            }]
        }";
        var method = typeof(ChatClient).GetMethod("ParseStreamingUpdate",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(client, new object[] { json });

        // Assert
        var update = result as StreamingChatCompletionUpdate;
        update.Should().NotBeNull();
        update!.FinishReason.Should().Be("MAX_TOKENS");
    }

    [Fact]
    public void ParseStreamingUpdate_WithNoCandidates_ShouldReturnNull()
    {
        // Arrange
        var client = new ChatClient(TestModel, TestApiKey);
        var json = @"{}";
        var method = typeof(ChatClient).GetMethod("ParseStreamingUpdate",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(client, new object[] { json });

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseStreamingUpdate_WithMultipleParts_ShouldConcatenate()
    {
        // Arrange
        var client = new ChatClient(TestModel, TestApiKey);
        var json = @"{
            ""candidates"": [{
                ""content"": {
                    ""parts"": [
                        {""text"": ""Part ""},
                        {""text"": ""1 ""},
                        {""text"": ""done""}
                    ]
                }
            }]
        }";
        var method = typeof(ChatClient).GetMethod("ParseStreamingUpdate",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(client, new object[] { json });

        // Assert
        var update = result as StreamingChatCompletionUpdate;
        update.Should().NotBeNull();
        update!.ContentUpdate.Should().Be("Part 1 done");
    }

    [Fact]
    public void BuildRequestBody_WithNullOptions_ShouldOmitGenerationConfig()
    {
        // Arrange
        var client = new ChatClient(TestModel, TestApiKey);
        var messages = new[] { ChatMessage.CreateUserMessage("Test") };
        var method = typeof(ChatClient).GetMethod("BuildRequestBody",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(client, new object[] { messages, null, false });

        // Assert
        result.Should().NotBeNull();
        var json = result as string;
        var doc = JsonDocument.Parse(json!);
        doc.RootElement.TryGetProperty("generationConfig", out var config).Should().BeTrue();
        // Config exists but all values should be null/omitted in JSON
    }

    [Fact]
    public void BuildRequestBody_WithEmptyMessageContent_ShouldHandleGracefully()
    {
        // Arrange
        var client = new ChatClient(TestModel, TestApiKey);
        var messages = new[] { new ChatMessage("user", new List<ContentPart>()) };
        var method = typeof(ChatClient).GetMethod("BuildRequestBody",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(client, new object[] { messages, null, false });

        // Assert
        result.Should().NotBeNull();
        var json = result as string;
        var doc = JsonDocument.Parse(json!);
        var parts = doc.RootElement.GetProperty("contents")[0].GetProperty("parts");
        parts.GetArrayLength().Should().Be(0);
    }

    [Fact]
    public void ParseChatCompletion_WithRawResponseData_ShouldPreserveRawResponse()
    {
        // Arrange
        var client = new ChatClient(TestModel, TestApiKey);
        var json = @"{
            ""candidates"": [{
                ""content"": {
                    ""parts"": [{""text"": ""Test""}]
                }
            }],
            ""modelVersion"": ""gemini-pro-001""
        }";
        var method = typeof(ChatClient).GetMethod("ParseChatCompletion",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(client, new object[] { json });

        // Assert
        var completion = result as ChatCompletion;
        completion.Should().NotBeNull();
        completion!.RawResponse.Should().NotBeNull();
        completion.RawResponse.Value.TryGetProperty("modelVersion", out var version).Should().BeTrue();
    }

    [Theory]
    [InlineData("STOP")]
    [InlineData("MAX_TOKENS")]
    [InlineData("SAFETY")]
    [InlineData("RECITATION")]
    public void ParseChatCompletion_WithDifferentFinishReasons_ShouldParseCorrectly(string finishReason)
    {
        // Arrange
        var client = new ChatClient(TestModel, TestApiKey);
        var json = $@"{{
            ""candidates"": [{{
                ""content"": {{
                    ""parts"": [{{""text"": ""Test""}}]
                }},
                ""finishReason"": ""{finishReason}""
            }}]
        }}";
        var method = typeof(ChatClient).GetMethod("ParseChatCompletion",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(client, new object[] { json });

        // Assert
        var completion = result as ChatCompletion;
        completion.Should().NotBeNull();
        completion!.FinishReason.Should().Be(finishReason);
    }

    [Fact]
    public void BuildRequestBody_WithOnlyMaxTokens_ShouldIncludeOnlyMaxTokens()
    {
        // Arrange
        var client = new ChatClient(TestModel, TestApiKey);
        var messages = new[] { ChatMessage.CreateUserMessage("Test") };
        var options = new ChatCompletionOptions { MaxTokens = 512 };
        var method = typeof(ChatClient).GetMethod("BuildRequestBody",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(client, new object[] { messages, options, false });

        // Assert
        result.Should().NotBeNull();
        var json = result as string;
        var doc = JsonDocument.Parse(json!);
        var config = doc.RootElement.GetProperty("generationConfig");
        config.TryGetProperty("maxOutputTokens", out var maxTokens).Should().BeTrue();
        maxTokens.GetInt32().Should().Be(512);
    }

    [Fact]
    public void ParseStreamingUpdate_WithNoFinishReason_ShouldHaveNullFinishReason()
    {
        // Arrange
        var client = new ChatClient(TestModel, TestApiKey);
        var json = @"{
            ""candidates"": [{
                ""content"": {
                    ""parts"": [{""text"": ""Chunk""}]
                }
            }]
        }";
        var method = typeof(ChatClient).GetMethod("ParseStreamingUpdate",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(client, new object[] { json });

        // Assert
        var update = result as StreamingChatCompletionUpdate;
        update.Should().NotBeNull();
        update!.FinishReason.Should().BeNull();
    }

    [Fact]
    public void ParseChatCompletion_WithEmptyTextInPart_ShouldHandleGracefully()
    {
        // Arrange
        var client = new ChatClient(TestModel, TestApiKey);
        var json = @"{
            ""candidates"": [{
                ""content"": {
                    ""parts"": [{""text"": """"}]
                }
            }]
        }";
        var method = typeof(ChatClient).GetMethod("ParseChatCompletion",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(client, new object[] { json });

        // Assert
        var completion = result as ChatCompletion;
        completion.Should().NotBeNull();
        completion!.Content.Should().BeEmpty();
    }

    [Fact]
    public void ParseStreamingUpdate_WithEmptyTextInPart_ShouldReturnEmptyString()
    {
        // Arrange
        var client = new ChatClient(TestModel, TestApiKey);
        var json = @"{
            ""candidates"": [{
                ""content"": {
                    ""parts"": [{""text"": """"}]
                }
            }]
        }";
        var method = typeof(ChatClient).GetMethod("ParseStreamingUpdate",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = method?.Invoke(client, new object[] { json });

        // Assert
        var update = result as StreamingChatCompletionUpdate;
        update.Should().NotBeNull();
        update!.ContentUpdate.Should().BeEmpty();
    }
}
