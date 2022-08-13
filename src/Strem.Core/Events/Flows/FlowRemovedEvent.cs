namespace Strem.Core.Events.Flows;

public class FlowRemovedEvent
{
    public Guid FlowId { get; }

    public FlowRemovedEvent(Guid flowId)
    {
        FlowId = flowId;
    }
}