using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;

namespace Strem.Twitch.Flows.Tasks.Channels;

public class LeaveTwitchChannelTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "leave-twitch-channel";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public string Channel { get; set; }
}