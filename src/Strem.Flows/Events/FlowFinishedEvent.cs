using Strem.Flows.Events.Base;

namespace Strem.Flows.Events;

public record FlowFinishedEvent(Guid FlowId) : FlowEvent(FlowId);