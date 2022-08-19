using Strem.Core.Flows.Tasks;
using Strem.Core.Types;

namespace Strem.Twitch.Flows.Tasks.Chat;

public class SetTwitchSubscriberOnlyChatTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "set-twitch-subscriber-only-chat";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string Channel { get; set; }
    public bool SetSubscriberOnlyChat { get; set; }
}