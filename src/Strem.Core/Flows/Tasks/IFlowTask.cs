using Strem.Core.Variables;

namespace Strem.Core.Flows.Tasks;

public interface IFlowTask : IFlowElement
{
    bool CanExecute();
    Task<bool> Execute(object data, IVariables flowVars);
}