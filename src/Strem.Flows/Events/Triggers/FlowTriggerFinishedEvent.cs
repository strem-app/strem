using Strem.Flows.Events.Base;

namespace Strem.Flows.Events.Triggers;

public record FlowTriggerFinishedEvent(Guid FlowId, Guid TriggerId) : FlowTriggerEvent(FlowId, TriggerId);