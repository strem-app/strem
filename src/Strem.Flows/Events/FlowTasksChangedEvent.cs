namespace Strem.Flows.Events;

public record FlowTaskChangedEvent(Guid FlowId, Guid TaskId);