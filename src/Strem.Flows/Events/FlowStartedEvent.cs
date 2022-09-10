using Strem.Flows.Events.Base;

namespace Strem.Flows.Events;

public record FlowStartedEvent(Guid FlowId) : FlowEvent(FlowId);