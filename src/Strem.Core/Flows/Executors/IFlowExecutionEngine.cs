namespace Strem.Core.Flows.Executors;

public interface IFlowExecutionEngine : IFlowExecutor, IDisposable
{
    void StartEngine();
    void SetupFlow(Flow flow);
    void RemoveFlow(Guid flowId);
    IReadOnlyCollection<FlowExecutionLog> ExecutionLogs { get; }
}