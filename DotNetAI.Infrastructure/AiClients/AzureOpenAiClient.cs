using DotNetAI.AI.Abstractions;
using DotNetAI.AI.Abstractions.Models;

namespace DotNetAI.Infrastructure.AiClients;

public class AzureOpenAiClient : IAiClient
{
    public AiResult Execute(AiRequest request)
    {
        return new AiResult
        {
            Output = "Stub response from Azure OpenAI client"
        };
    }
}
