namespace Strem.Core.Events.Flows;

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