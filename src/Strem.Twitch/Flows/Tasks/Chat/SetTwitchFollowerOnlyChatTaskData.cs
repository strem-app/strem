using Strem.Core.Flows.Tasks;
using Strem.Core.Types;

namespace Strem.Twitch.Flows.Tasks.Chat;

public class SetTwitchFollowerOnlyChatTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "set-twitch-follower-only-chat";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string Channel { get; set; }
    public bool SetFollowerOnlyChat { get; set; }
    public TimeUnitType FollowerTimeUnit { get; set; }
    public string FollowerTimeValue { get; set; }
}