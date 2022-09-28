using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;

namespace Strem.OBS.Flows.Tasks;

public class ChangeSceneTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "set-obs-change-scene";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public string NewScene { get; set; }
}