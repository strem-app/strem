using Strem.Core.Flows.Triggers;
using Strem.Core.Types;

namespace Strem.Flows.Default.Flows.Triggers.Data;

public class OnIntervalTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-interval";
    public static readonly string TriggerVersion = "1.0.0";
    
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;
    
    public bool StartImmediately { get; set; }
    public string IntervalValue { get; set; }
    public TimeUnit IntervalUnits { get; set; }
}