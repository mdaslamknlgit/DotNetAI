using DotNetAI.AI.Abstractions;
using DotNetAI.AI.Abstractions.Models;

namespace DotNetAI.Application.Agents;

public class SimpleAiAgent : IAiAgent
{
    private readonly IAiClient _aiClient;

    public SimpleAiAgent(IAiClient aiClient)
    {
        _aiClient = aiClient;
    }

    public string Run(string input)
    {
        var prompt = $"""
            You are an AI agent.
            Think step by step and answer clearly.

            User input:
            {input}
            """;

        var result = _aiClient.Execute(new AiRequest
        {
            Prompt = prompt
        });

        return result.Output;
    }
}
