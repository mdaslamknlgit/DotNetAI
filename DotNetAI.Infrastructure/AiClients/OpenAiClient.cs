using System.Net.Http;
using System.Text;
using System.Text.Json;
using DotNetAI.AI.Abstractions;
using DotNetAI.AI.Abstractions.Models;

namespace DotNetAI.Infrastructure.AiClients;

public class OpenAiClient : IAiClient
{
    public AiResult Execute(AiRequest request)
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OPENAI_API_KEY is not set");

        using var http = new HttpClient();

        http.DefaultRequestHeaders.Add(
            "Authorization",
            $"Bearer {apiKey}"
        );

        var body = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "user", content = request.Prompt }
            }
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = http.PostAsync(
            "https://api.openai.com/v1/chat/completions",
            content
        ).Result;

        var responseJson = response.Content.ReadAsStringAsync().Result;
        using var doc = JsonDocument.Parse(responseJson);

        var output =
            doc.RootElement
               .GetProperty("choices")[0]
               .GetProperty("message")
               .GetProperty("content")
               .GetString();

        return new AiResult
        {
            Output = output ?? string.Empty
        };
    }
}
