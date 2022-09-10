using Strem.Flows.Data.Tasks;

namespace Strem.Twitch.Flows.Tasks.Clips;

public class CreateTwitchClipTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "create-twitch-clip";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string Channel { get; set; }
    public bool CreateTodo { get; set; }
    public List<string> Tags { get; set; } = new();
}