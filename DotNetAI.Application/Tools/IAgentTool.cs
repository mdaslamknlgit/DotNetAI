namespace DotNetAI.Application.Tools;

public interface IAgentTool
{
    string Name { get; }
    string Description { get; }

    string Execute(string input);
}
