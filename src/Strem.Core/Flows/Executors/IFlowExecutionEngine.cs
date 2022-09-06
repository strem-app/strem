namespace Strem.Core.Flows.Executors;

public interface IFlowExecutionEngine : IFlowExecutor, IDisposable
{
    Task StartEngine();
}