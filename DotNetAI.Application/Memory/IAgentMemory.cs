namespace DotNetAI.Application.Memory;

public interface IAgentMemory
{
    IReadOnlyList<string> GetHistory();
    void Add(string entry);
    void Clear();
}
