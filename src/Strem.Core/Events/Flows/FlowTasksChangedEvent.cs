namespace Strem.Core.Events.Flows;

public record FlowTaskChangedEvent(Guid FlowId, Guid TaskId);