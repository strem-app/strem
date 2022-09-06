namespace Strem.Core.Events.Flows;

public record FlowTriggerChangedEvent(Guid FlowId, Guid TriggerId);