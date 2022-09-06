using Strem.Core.Flows.Executors.Logging;
using Strem.Core.Flows.Tasks;
using Strem.Core.Variables;

namespace Strem.Core.Flows.Executors;

public interface ITaskExecutor
{
    Task<ExecutionResult> ExecuteTask(Flow flow, IFlowTaskData taskData, IVariables flowVariables, FlowExecutionLog executionLog);
}