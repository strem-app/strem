namespace Strem.Core.Events;

public class FlowChangedEvent
{
    public Guid FlowId { get; }

    public FlowChangedEvent(Guid flowId)
    {
        FlowId = flowId;
    }
}