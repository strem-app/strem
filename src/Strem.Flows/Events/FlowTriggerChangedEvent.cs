namespace Strem.Flows.Events;

public record FlowTriggerChangedEvent(Guid FlowId, Guid TriggerId);