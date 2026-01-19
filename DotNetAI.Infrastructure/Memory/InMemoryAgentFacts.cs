using DotNetAI.Application.Memory;

namespace DotNetAI.Infrastructure.Memory;

public class InMemoryAgentFacts : IAgentFacts
{
    private readonly Dictionary<string, string> _facts = new();

    public void Set(string key, string value)
    {
        _facts[key] = value;
    }

    public bool TryGet(string key, out string value)
    {
        return _facts.TryGetValue(key, out value!);
    }

    public IReadOnlyDictionary<string, string> GetAll()
    {
        return _facts;
    }
}
