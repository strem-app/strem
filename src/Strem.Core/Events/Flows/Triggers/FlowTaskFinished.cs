namespace Strem.Core.Events.Flows.Triggers;

public class FlowTriggerFinished
{
    public Guid FlowId { get; }
    public Guid TriggerId { get; }

    public FlowTriggerFinished(Guid flowId, Guid triggerId)
    {
        FlowId = flowId;
        TriggerId = triggerId;
    }
}