using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using DotNetAI.AI.Abstractions;
using DotNetAI.AI.Abstractions.Models;
using DotNetAI.Infrastructure.Configuration;

namespace DotNetAI.Infrastructure.AiClients;

public class OpenAiClient : IAiClient
{
    private readonly HttpClient _http;
    private readonly OpenAiOptions _options;

    public OpenAiClient(HttpClient http, IOptions<OpenAiOptions> options)
    {
        _http = http;
        _options = options.Value;

        _http.BaseAddress = new Uri(_options.BaseUrl);
        _http.DefaultRequestHeaders.Clear();
        _http.DefaultRequestHeaders.Add(
            "Authorization",
            $"Bearer {_options.ApiKey}"
        );
    }

    public AiResult Execute(AiRequest request)
    {
        var body = new
        {
            model = _options.Model,
            input = request.Prompt
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = _http.PostAsync("/v1/responses", content).Result;
        var responseJson = response.Content.ReadAsStringAsync().Result;

        using var doc = JsonDocument.Parse(responseJson);
        var root = doc.RootElement;

        // 1️⃣ output_text (when available)
        if (root.TryGetProperty("output_text", out var outputText))
        {
            return new AiResult
            {
                Output = outputText.GetString() ?? string.Empty
            };
        }

        // 2️⃣ output[].content[].text (most common)
        if (root.TryGetProperty("output", out var outputArray))
        {
            foreach (var output in outputArray.EnumerateArray())
            {
                if (output.TryGetProperty("content", out var contentArray))
                {
                    foreach (var item in contentArray.EnumerateArray())
                    {
                        if (item.TryGetProperty("text", out var text))
                        {
                            return new AiResult
                            {
                                Output = text.GetString() ?? string.Empty
                            };
                        }
                    }
                }
            }
        }

        // 3️⃣ Fallback: dump for debugging
        throw new InvalidOperationException(
            "Unable to extract text from OpenAI response:\n" + responseJson
        );
    }
}
