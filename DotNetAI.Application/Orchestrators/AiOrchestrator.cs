using DotNetAI.Application.Models;
using DotNetAI.AI.Abstractions;
using DotNetAI.AI.Abstractions.Models;

namespace DotNetAI.Application.Orchestrators;

public class AiOrchestrator
{
    private readonly IAiClient _aiClient;

    public AiOrchestrator(IAiClient aiClient)
    {
        _aiClient = aiClient;
    }

    public AiResponse Handle(string userInput)
    {
        var request = new AiRequest
        {
            Prompt = userInput
        };

        var result = _aiClient.Execute(request);

        return new AiResponse
        {
            Content = result.Output
        };
    }
}
