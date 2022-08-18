using Strem.Core.Flows.Tasks;

namespace Strem.Twitch.Flows.Tasks.Chat;

public class ClearTwitchChatTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "clear-twitch-chat";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string Channel { get; set; }
}