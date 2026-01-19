using DotNetAI.Application.Memory;

namespace DotNetAI.Infrastructure.Memory;

public class InMemoryAgentMemory : IAgentMemory
{
    private readonly List<string> _history = new();

    public IReadOnlyList<string> GetHistory()
        => _history.AsReadOnly();

    public void Add(string entry)
    {
        _history.Add(entry);
    }

    public void Clear()
    {
        _history.Clear();
    }
}
