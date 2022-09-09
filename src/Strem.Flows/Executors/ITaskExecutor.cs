using Strem.Core.Variables;
using Strem.Flows.Data;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Executors.Logging;

namespace Strem.Flows.Executors;

public interface ITaskExecutor
{
    Task<ExecutionResult> ExecuteTask(Flow flow, IFlowTaskData taskData, IVariables flowVariables, FlowExecutionLog executionLog);
}