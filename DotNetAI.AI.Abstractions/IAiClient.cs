using DotNetAI.AI.Abstractions.Models;

namespace DotNetAI.AI.Abstractions;

public interface IAiClient
{
    AiResult Execute(AiRequest request);
}
