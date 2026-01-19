using DotNetAI.AI.Abstractions;
using DotNetAI.AI.Abstractions.Models;
using System.Text.Json;

namespace DotNetAI.Application.Memory;

public class LlmFactExtractor
{
    private readonly IAiClient _aiClient;

    public LlmFactExtractor(IAiClient aiClient)
    {
        _aiClient = aiClient;
    }

    public Dictionary<string, string> Extract(string input)
    {
        var prompt = $"""
        Extract personal facts from the following text.

        Rules:
        - Output ONLY valid JSON
        - Keys must be snake_case
        - Values must be strings
        - If no facts found, return empty JSON: {{}}

        Text:
        {input}

        Example output:
        {{ "name": "Mohammed Aslam", "father_name": "Ahmed" }}
        """;

        var response = _aiClient.Execute(new AiRequest
        {
            Prompt = prompt
        }).Output;

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(response)
                   ?? new Dictionary<string, string>();
        }
        catch
        {
            // If model misbehaves, ignore safely
            return new Dictionary<string, string>();
        }
    }
}
