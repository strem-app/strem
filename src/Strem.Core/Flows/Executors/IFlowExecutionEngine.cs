namespace Strem.Core.Flows.Executors;

public interface IFlowExecutionEngine : IFlowExecutor, IDisposable
{
    Task StartEngine();
    Task SetupFlow(Flow flow);
    void RemoveFlow(Guid flowId);
    IReadOnlyCollection<FlowExecutionLog> ExecutionLogs { get; }
}