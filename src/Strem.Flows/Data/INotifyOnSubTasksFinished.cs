using Strem.Core.Variables;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Types;

namespace Strem.Flows.Data;

/// <summary>
/// This interface allows you to run post processing after subtasks finished
/// </summary>
public interface INotifyOnSubTasksFinished
{
    /// <summary>
    /// This is triggered when the sub tasks have finished executing
    /// </summary>
    /// <returns>A task wrapping a true for success a false for failure</returns>
    Task OnSubTasksFinished(IFlowTaskData data, IVariables flowVars, ExecutionResultType executionResultType);
}