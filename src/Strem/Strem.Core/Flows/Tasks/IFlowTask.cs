using Strem.Core.Variables;

namespace Strem.Core.Flows.Tasks;

public interface IFlowTask : IFlowElement
{
    Task Execute(object data, IVariables flowVars);
}