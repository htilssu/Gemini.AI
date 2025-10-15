using System.ClientModel.Primitives;

namespace Gemini.Content;

public class GeminiClientOptions : ClientPipelineOptions
{
    private Uri? _endpoint;

    public Uri? Endpoint
    {
        get => _endpoint;
        set
        {
            AssertNotFrozen();
            _endpoint = value;
        }
    }
}