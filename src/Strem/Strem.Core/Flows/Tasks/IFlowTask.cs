using Strem.Core.Variables;

namespace Strem.Core.Flows.Tasks;

public interface IFlowTask<in T> where T : IFlowTaskData
{
    string TaskVersion { get; }
    string TaskCode { get; }
    string Name { get; }
    string Description { get; }
    
    Task Execute(T data, IVariables flowVars);
}