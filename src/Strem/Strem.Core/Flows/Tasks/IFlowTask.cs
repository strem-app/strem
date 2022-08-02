using Strem.Core.Variables;

namespace Strem.Core.Flows.Tasks;

public interface IFlowTask : IFlowElement {}

public interface IFlowTask<in T>: IFlowTask 
    where T : IFlowTaskData
{
    Task Execute(T data, IVariables flowVars);
}