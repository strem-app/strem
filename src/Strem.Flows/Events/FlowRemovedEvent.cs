using Strem.Flows.Events.Base;

namespace Strem.Flows.Events;

public record FlowRemovedEvent(Guid FlowId) : FlowEvent(FlowId);