namespace Strem.Core.Events.Flows;

public class FlowEnabledEvent
{
    public Guid FlowId { get; }

    public FlowEnabledEvent(Guid flowId)
    {
        FlowId = flowId;
    }
}