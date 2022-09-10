namespace Strem.Flows.Events.Base;

public abstract record FlowTriggerEvent(Guid FlowId, Guid TriggerId) : FlowEvent(FlowId);