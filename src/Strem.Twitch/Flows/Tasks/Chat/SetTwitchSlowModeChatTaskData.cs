using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;
using Strem.Core.Types;

namespace Strem.Twitch.Flows.Tasks.Chat;

public class SetTwitchSlowModeChatTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "set-twitch-slow-mode-chat";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string Channel { get; set; }
    [Required]
    public bool SetSlowMode { get; set; }
    public TimeUnitType TimeoutUnit { get; set; }
    public string TimeoutValue { get; set; }
}