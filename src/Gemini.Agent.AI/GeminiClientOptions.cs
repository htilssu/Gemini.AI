using System.ClientModel.Primitives;

namespace Gemini.Agent.AI;

public class GeminiClientOptions : ClientPipelineOptions
{
    private Uri _endpoint;
    
    public Uri Endpoint
    {
        get => _endpoint;
        set
        {
            AssertNotFrozen();
            _endpoint = value;
        }
    }
}