namespace Strem.Core.Events.Flows.Triggers;

public class FlowTriggerStarted
{
    public Guid FlowId { get; }
    public Guid TriggerId { get; }

    public FlowTriggerStarted(Guid flowId, Guid triggerId)
    {
        FlowId = flowId;
        TriggerId = triggerId;
    }
}