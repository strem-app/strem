using Strem.Core.Flows.Triggers;
using Strem.Core.Utils;
using Strem.Flows.Default.Flows.Tasks;

namespace Strem.Flows.Default.Flows.Triggers.Data;

public class OnIntervalTriggerData : IFlowTriggerData
{
    public string Code => OnIntervalTrigger.TriggerCode;
    public string Version { get; set; } = OnIntervalTrigger.TriggerVersion;
    
    public bool StartImmediately { get; set; }
    public int IntervalValue { get; set; }
    public TimeUnit IntervalUnits { get; set; }
}