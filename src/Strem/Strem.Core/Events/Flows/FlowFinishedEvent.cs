namespace Strem.Core.Events.Flows;

public class FlowFinishedEvent
{
    public Guid FlowId { get; }

    public FlowFinishedEvent(Guid flowId)
    {
        FlowId = flowId;
    }
}