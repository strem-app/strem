namespace Strem.Flows.Events.Base;

public abstract record FlowTaskEvent(Guid FlowId, Guid TriggerId) : FlowEvent(FlowId);