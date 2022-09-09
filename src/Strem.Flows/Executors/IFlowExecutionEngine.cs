namespace Strem.Flows.Executors;

public interface IFlowExecutionEngine : IFlowExecutor, IDisposable
{
    Task StartEngine();
}