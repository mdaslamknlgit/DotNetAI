using DotNetAI.AI.Abstractions;
using DotNetAI.AI.Abstractions.Models;
using DotNetAI.Application.Tools;

namespace DotNetAI.Application.Agents;

public class ToolAwareAgent : IAiAgent
{
    private readonly IAiClient _aiClient;
    private readonly IEnumerable<IAgentTool> _tools;

    public ToolAwareAgent(
        IAiClient aiClient,
        IEnumerable<IAgentTool> tools)
    {
        _aiClient = aiClient;
        _tools = tools;
    }

    public string Run(string input)
    {
        var toolList = string.Join("\n", _tools.Select(t =>
            $"- {t.Name}: {t.Description}"
        ));

        var agentPrompt = $"""
You are an AI agent.

Available tools:
{toolList}

Rules:
- If a tool is needed, respond ONLY in this format:
  TOOL:<tool_name>|<input>
- Otherwise, respond normally.

User:
{input}
""";

        var response = _aiClient.Execute(new AiRequest
        {
            Prompt = agentPrompt
        }).Output;

        // Tool decision
        if (response.StartsWith("TOOL:"))
        {
            var parts = response.Replace("TOOL:", "").Split('|');
            var toolName = parts[0];
            var toolInput = parts.Length > 1 ? parts[1] : "";

            var tool = _tools.First(t => t.Name == toolName);
            var toolResult = tool.Execute(toolInput);

            // Final answer after tool execution
            var finalPrompt = $"""
                Tool result:
                {toolResult}

                Explain this to the user clearly.
                """;

            return _aiClient.Execute(new AiRequest
            {
                Prompt = finalPrompt
            }).Output;
        }

        return response;
    }
}
