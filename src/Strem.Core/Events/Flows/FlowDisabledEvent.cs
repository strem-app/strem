namespace Strem.Core.Events.Flows;

public class FlowDisabledEvent
{
    public Guid FlowId { get; }

    public FlowDisabledEvent(Guid flowId)
    {
        FlowId = flowId;
    }
}