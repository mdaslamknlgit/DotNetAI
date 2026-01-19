using DotNetAI.Application.Agents;
using DotNetAI.Application.Models;

namespace DotNetAI.Application.Orchestrators;

public class AiOrchestrator
{
    private readonly IAiAgent _agent;

    public AiOrchestrator(IAiAgent agent)
    {
        _agent = agent;
    }

    public AiResponse Handle(string userInput)
    {
        var output = _agent.Run(userInput);

        return new AiResponse
        {
            Content = output
        };
    }
}
