using System.ComponentModel.DataAnnotations;
using Strem.Core.Flows.Tasks;
using Strem.OBS.v4.Types;

namespace Strem.OBS.v4.Flows.Tasks;

public class SetSourceVisibilityTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "set-obs-v4-source-visibility";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public string SceneName { get; set; }
    
    [Required]
    public string SourceName { get; set; }
    
    [Required]
    public VisibilityStatus Status { get; set; }
}