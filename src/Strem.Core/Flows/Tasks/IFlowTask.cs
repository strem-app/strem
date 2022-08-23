using Strem.Core.Flows.Executors;
using Strem.Core.Variables;

namespace Strem.Core.Flows.Tasks;

public interface IFlowTask : IFlowElement
{
    bool CanExecute();
    Task<ExecutionResult> Execute(object data, IVariables flowVars);
}