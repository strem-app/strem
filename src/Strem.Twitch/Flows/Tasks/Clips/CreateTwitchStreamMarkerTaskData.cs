using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;

namespace Strem.Twitch.Flows.Tasks.Clips;

public class CreateTwitchStreamMarkerTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "create-twitch-stream-marker";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public string Description { get; set; }
}