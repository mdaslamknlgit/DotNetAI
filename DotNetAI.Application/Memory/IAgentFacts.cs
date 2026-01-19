namespace DotNetAI.Application.Memory;

public interface IAgentFacts
{
    void Set(string key, string value);
    bool TryGet(string key, out string value);
    IReadOnlyDictionary<string, string> GetAll();
}
