using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;
using TwitchLib.Client.Enums;

namespace Strem.Twitch.Flows.Tasks.Stream;

public class PlayTwitchCommercialTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "play-twitch-commercial";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string Channel { get; set; }
    
    [Required]
    public CommercialLength CommercialLength { get; set; }
}