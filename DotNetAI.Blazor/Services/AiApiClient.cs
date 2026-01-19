using System.Net.Http.Json;
using DotNetAI.Blazor.Models;

namespace DotNetAI.Blazor.Services;

public class AiApiClient
{
    private readonly HttpClient _http;

    public AiApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<AiResponseDto?> AskAsync(string prompt)
    {
        var request = new AiRequestDto
        {
            Prompt = prompt
        };

        var response = await _http.PostAsJsonAsync("api/ai", request);
        return await response.Content.ReadFromJsonAsync<AiResponseDto>();
    }
}
