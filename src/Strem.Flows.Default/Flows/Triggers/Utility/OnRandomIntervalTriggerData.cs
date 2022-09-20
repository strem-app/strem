using System.ComponentModel.DataAnnotations;
using Strem.Core.Types;
using Strem.Flows.Data.Triggers;

namespace Strem.Flows.Default.Flows.Triggers.Utility;

public class OnRandomIntervalTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-random-interval";
    public static readonly string TriggerVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;

    [Required]
    public string MinIntervalValue { get; set; } = string.Empty;
    public TimeUnitType MinIntervalUnitsType { get; set; }
    
    [Required]
    public string MaxIntervalValue { get; set; } = string.Empty;
    public TimeUnitType MaxIntervalUnitsType { get; set; }
}