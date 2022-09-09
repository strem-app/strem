using Strem.Core.Variables;
using Strem.Flows.Executors;

namespace Strem.Flows.Data.Tasks;

public interface IFlowTask : IFlowElement
{
    bool CanExecute();
    Task<ExecutionResult> Execute(object data, IVariables flowVars);
}