using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;
using Strem.OBS.Types;

namespace Strem.OBS.Flows.Tasks;

public class SetInputVolumeTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "set-obs-input-volume";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;

    [Required]
    public string InputName { get; set; }
    
    public string Volume { get; set; }
}