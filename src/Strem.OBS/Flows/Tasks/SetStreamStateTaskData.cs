using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;

namespace Strem.OBS.Flows.Tasks;

public class SetStreamStateTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "set-obs-stream-state";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public bool StartStream { get; set; }
}