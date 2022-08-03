namespace Strem.Core.Events.Flows;

public class FlowTasksChangedEvent
{
    public Guid FlowId { get; }

    public FlowTasksChangedEvent(Guid flowId)
    {
        FlowId = flowId;
    }
}