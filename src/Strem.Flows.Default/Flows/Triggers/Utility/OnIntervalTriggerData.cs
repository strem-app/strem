using System.ComponentModel.DataAnnotations;
using Strem.Core.Flows.Triggers;
using Strem.Core.Types;

namespace Strem.Flows.Default.Flows.Triggers.Utility;

public class OnIntervalTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-interval";
    public static readonly string TriggerVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;
    
    public bool StartImmediately { get; set; }
    [Required]
    public string IntervalValue { get; set; } = string.Empty;
    public TimeUnitType IntervalUnitsType { get; set; }
}