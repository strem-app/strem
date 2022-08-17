using Strem.Core.Variables;

namespace Strem.Core.Flows.Executors;

public interface IFlowExecutor
{
    Task ExecuteFlow(Flow flow, IVariables flowVariables = null);
    Task ExecuteFlow(Guid flowId, IVariables flowVariables = null);
}