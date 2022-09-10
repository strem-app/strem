using Strem.Flows.Events.Base;

namespace Strem.Flows.Events;

public record FlowDetailsChangedEvent(Guid FlowId) : FlowEvent(FlowId);