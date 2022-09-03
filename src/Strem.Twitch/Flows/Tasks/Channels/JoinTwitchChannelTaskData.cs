using System.ComponentModel.DataAnnotations;
using Strem.Core.Flows.Tasks;

namespace Strem.Twitch.Flows.Tasks.Channels;

public class JoinTwitchChannelTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "join-twitch-channel";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public string Channel { get; set; }
}