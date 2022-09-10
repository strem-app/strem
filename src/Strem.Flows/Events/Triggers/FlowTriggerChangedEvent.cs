using Strem.Flows.Events.Base;

namespace Strem.Flows.Events.Triggers;

public record FlowTriggerChangedEvent(Guid FlowId, Guid TriggerId) : FlowTriggerEvent(FlowId, TriggerId);