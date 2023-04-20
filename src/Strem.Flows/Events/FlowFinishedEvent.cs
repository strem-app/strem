using Strem.Flows.Events.Base;
using Strem.Flows.Types;

namespace Strem.Flows.Events;

public record FlowFinishedEvent(Guid FlowId, ExecutionResultType ExecutionResultType) : FlowEvent(FlowId);