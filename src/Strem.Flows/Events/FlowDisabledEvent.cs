using Strem.Flows.Events.Base;

namespace Strem.Flows.Events;

public record FlowDisabledEvent(Guid FlowId) : FlowEvent(FlowId);