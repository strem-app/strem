using Strem.Core.Flows.Triggers;

namespace Strem.Flows.Default.Flows.Triggers.Data;

public class OnEventRaisedTriggerData : IFlowTriggerData
{
    public string Code => OnEventRaisedTrigger.TriggerCode;
    public string Version { get; set; } = OnEventRaisedTrigger.TriggerVersion;
    
    public string EventName { get; set; }
}