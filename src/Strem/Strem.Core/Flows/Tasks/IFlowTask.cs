using Strem.Core.Variables;

namespace Strem.Core.Flows.Tasks;

public interface IFlowTask : IFlowElement
{
    bool CanExecute();
    Task Execute(object data, IVariables flowVars);
}