namespace Strem.Core.Events;

public class FlowRemovedEvent
{
    public Guid FlowId { get; }

    public FlowRemovedEvent(Guid flowId)
    {
        FlowId = flowId;
    }
}