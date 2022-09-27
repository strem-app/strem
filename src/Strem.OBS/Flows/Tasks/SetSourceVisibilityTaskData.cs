using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;
using Strem.OBS.Types;

namespace Strem.OBS.Flows.Tasks;

public class SetSourceVisibilityTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "set-obs-source-visibility";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;

    [Required]
    public string SceneName { get; set; }
    
    [Required]
    public string SceneItemName { get; set; }
    
    [Required]
    public VisibilityStatus Status { get; set; }
}