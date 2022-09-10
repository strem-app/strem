using Strem.Flows.Events.Base;

namespace Strem.Flows.Events;

public record FlowEnabledEvent(Guid FlowId) : FlowEvent(FlowId);