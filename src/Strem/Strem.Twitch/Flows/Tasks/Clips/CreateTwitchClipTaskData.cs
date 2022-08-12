using Strem.Core.Flows.Tasks;

namespace Strem.Twitch.Flows.Tasks.Chat;

public class CreateTwitchClipTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "send-twitch-clip";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string Channel { get; set; }
}