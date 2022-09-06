namespace Strem.Core.Events.Flows.Tasks;

public record FlowTaskFinished(Guid FlowId, Guid TaskId);