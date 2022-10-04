using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;

namespace Strem.Flows.Default.Flows.Tasks.Flows;

public class DisableFlowTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "disable-flow";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public Guid FlowId { get; set; }
}