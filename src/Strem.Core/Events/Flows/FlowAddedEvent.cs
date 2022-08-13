namespace Strem.Core.Events.Flows;

public class FlowAddedEvent
{
    public Guid FlowId { get; }

    public FlowAddedEvent(Guid flowId)
    {
        FlowId = flowId;
    }
}