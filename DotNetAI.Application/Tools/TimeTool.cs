namespace DotNetAI.Application.Tools;

public class TimeTool : IAgentTool
{
    public string Name => "get_current_time";
    public string Description => "Returns the current server time";

    public string Execute(string input)
    {
        return DateTime.UtcNow.ToString("u");
    }
}
