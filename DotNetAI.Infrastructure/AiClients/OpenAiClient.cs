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

        var response = _http
            .PostAsync("/v1/responses", content)
            .Result;

        var responseJson = response.Content.ReadAsStringAsync().Result;

        using var doc = JsonDocument.Parse(responseJson);
        var root = doc.RootElement;

        // Correct Responses API parsing
        if (root.TryGetProperty("output", out var outputArray) &&
            outputArray.GetArrayLength() > 0 &&
            outputArray[0].TryGetProperty("content", out var contentArray) &&
            contentArray.GetArrayLength() > 0 &&
            contentArray[0].TryGetProperty("text", out var text))
        {
            return new AiResult
            {
                Output = text.GetString() ?? string.Empty
            };
        }

        throw new InvalidOperationException("Unable to extract text from OpenAI response");
    }
}
