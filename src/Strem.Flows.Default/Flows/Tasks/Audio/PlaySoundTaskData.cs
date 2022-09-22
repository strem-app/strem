using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;

namespace Strem.Flows.Default.Flows.Tasks.Audio;

public class PlaySoundTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "play-sound";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public string SoundFile { get; set; }
    
    [Required]
    [Range(1, 100, ErrorMessage = "{0} Must Be between 1 and 100")]
    public int Volume { get; set; } = 100;
}