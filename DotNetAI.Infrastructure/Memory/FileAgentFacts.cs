using DotNetAI.Application.Memory;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace DotNetAI.Infrastructure.Memory;

public class FileAgentFacts : IAgentFacts
{
    private readonly string _filePath;
    private readonly Dictionary<string, string> _facts;

    public FileAgentFacts(IHostEnvironment env)
    {
        _filePath = Path.Combine(env.ContentRootPath, "facts.json");

        // If file does not exist, start with empty facts
        if (!File.Exists(_filePath))
        {
            _facts = new Dictionary<string, string>();
            return;
        }

        // Read file
        var json = File.ReadAllText(_filePath);

        // If file is empty or whitespace, start fresh
        if (string.IsNullOrWhiteSpace(json))
        {
            _facts = new Dictionary<string, string>();
            return;
        }

        // Try to deserialize safely
        try
        {
            _facts = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                     ?? new Dictionary<string, string>();
        }
        catch
        {
            // Corrupted or invalid JSON → reset
            _facts = new Dictionary<string, string>();
        }
    }

    public void Set(string key, string value)
    {
        _facts[key] = value;
        Save();
    }

    public bool TryGet(string key, out string value)
    {
        return _facts.TryGetValue(key, out value!);
    }

    public IReadOnlyDictionary<string, string> GetAll()
    {
        return _facts;
    }

    private void Save()
    {
        var json = JsonSerializer.Serialize(
            _facts,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(_filePath, json);
    }
}
