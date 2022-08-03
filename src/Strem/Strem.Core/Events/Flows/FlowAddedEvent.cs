namespace Strem.Core.Events;

public class FlowAddedEvent
{
    public Guid FlowId { get; }

    public FlowAddedEvent(Guid flowId)
    {
        FlowId = flowId;
    }
}