using Strem.Flows.Events.Base;

namespace Strem.Flows.Events;

public record FlowAddedEvent(Guid FlowId) : FlowEvent(FlowId);