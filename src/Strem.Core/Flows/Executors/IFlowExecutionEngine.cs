using Strem.Core.Variables;

namespace Strem.Core.Flows.Executors;

public interface IFlowExecutionEngine : IDisposable
{
    void StartEngine();
    void SetupFlow(Flow flow);
    void RemoveFlow(Guid flowId);
    Task ExecuteFlow(Flow flow, IVariables flowVariables = null);
}