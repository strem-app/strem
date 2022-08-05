using Strem.Core.Flows.Triggers;

namespace Strem.Flows.Default.Flows.Triggers.Data;

public class OnEventRaisedTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-event-raised";
    public static readonly string TriggerVersion = "1.0.0";

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;
    
    public string EventName { get; set; }
}