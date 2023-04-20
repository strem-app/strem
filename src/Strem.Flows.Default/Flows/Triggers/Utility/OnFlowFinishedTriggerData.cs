using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Triggers;

namespace Strem.Flows.Default.Flows.Triggers.Utility;

public class OnFlowFinishedTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-flow-finished";
    public static readonly string TriggerVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;
    
    [Required]
    public Guid FlowId { get; set; }

    public bool OnlyOnSuccessfulExecution { get; set; } = true;
}