namespace Strem.Core.Events.Flows;

public class FlowStartedEvent
{
    public Guid FlowId { get; }

    public FlowStartedEvent(Guid flowId)
    {
        FlowId = flowId;
    }
}