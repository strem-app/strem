namespace Strem.Core.Events.Flows;

public class FlowTriggersChangedEvent
{
    public Guid FlowId { get; }

    public FlowTriggersChangedEvent(Guid flowId)
    {
        FlowId = flowId;
    }
}