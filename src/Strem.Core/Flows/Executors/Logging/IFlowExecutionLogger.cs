using Strem.Core.Flows.Tasks;
using Strem.Core.Types;
using Strem.Core.Variables;

namespace Strem.Core.Flows.Executors.Logging;

public interface IFlowExecutionLogger
{
    IReadOnlyCollection<FlowExecutionLog> ExecutionLogs { get; }
    FlowExecutionLog CreateLogFor(Flow flow, IVariables flowVariables);
    void CloseLogFor(FlowExecutionLog executionLog, IVariables flowVariables, ExecutionResultType resultType, IFlowTaskData? currentTaskData = null);
}