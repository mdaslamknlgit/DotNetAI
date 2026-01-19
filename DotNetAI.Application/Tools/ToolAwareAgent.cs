using DotNetAI.AI.Abstractions;
using DotNetAI.AI.Abstractions.Models;
using DotNetAI.Application.Memory;
using DotNetAI.Application.Tools;
using System;

namespace DotNetAI.Application.Agents;

public class ToolAwareAgent : IAiAgent
{
    private readonly IAiClient _aiClient;
    private readonly IEnumerable<IAgentTool> _tools;
    private readonly IAgentMemory _memory;
    private readonly IAgentFacts _facts;

    public ToolAwareAgent(
        IAiClient aiClient,
        IEnumerable<IAgentTool> tools,
        IAgentMemory memory,
        IAgentFacts facts)
    {
        _aiClient = aiClient;
        _tools = tools;
        _memory = memory;
        _facts = facts;
    }

    public string Run(string input)
    {
        // ✅ 1. Extract facts FIRST
        ExtractFacts(input);

        // ✅ 2. Store conversation
        _memory.Add($"User: {input}");

        // ✅ 3. Prepare facts for prompt
        var knownFacts = _facts.GetAll().Any()
            ? string.Join("\n", _facts.GetAll().Select(f => $"{f.Key}: {f.Value}"))
            : "None";

        var toolList = string.Join("\n", _tools.Select(t =>
            $"- {t.Name}: {t.Description}"
        ));

        // ✅ 4. Inject facts into prompt
        var agentPrompt = $"""
You are an AI agent.

Known facts about the user:
{knownFacts}

Conversation so far:
{string.Join("\n", _memory.GetHistory())}

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

        // ✅ 5. Tool handling
        if (response.StartsWith("TOOL:"))
        {
            var parts = response.Replace("TOOL:", "").Split('|');
            var toolName = parts[0];
            var toolInput = parts.Length > 1 ? parts[1] : "";

            var tool = _tools.First(t => t.Name == toolName);
            var toolResult = tool.Execute(toolInput);

            var finalPrompt = $"""
Known facts about the user:
{knownFacts}

Tool result:
{toolResult}

Explain this to the user clearly.
""";

            var finalResponse = _aiClient.Execute(new AiRequest
            {
                Prompt = finalPrompt
            }).Output;

            _memory.Add($"Agent: {finalResponse}");
            return finalResponse;
        }

        _memory.Add($"Agent: {response}");
        return response;
    }

    // ✅ Deterministic fact extraction
    private void ExtractFacts(string input)
    {
        var text = input.Trim();

        if (text.StartsWith("my name is ", StringComparison.OrdinalIgnoreCase))
        {
            var name = text.Substring(11).Trim();
            if (!string.IsNullOrEmpty(name))
            {
                _facts.Set("name", name);
            }
        }
    }
}
