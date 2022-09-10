using Strem.Core.Variables;
using Strem.Flows.Data;

namespace Strem.Flows.Executors;

public interface IFlowExecutor
{
    Task ExecuteFlow(Flow flow, IVariables flowVariables = null);
    Task ExecuteFlow(Guid flowId, IVariables flowVariables = null);
}