using Strem.Core.Variables;
using Strem.Flows.Data;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Types;

namespace Strem.Flows.Executors.Logging;

public interface IFlowExecutionLogger
{
    IReadOnlyCollection<FlowExecutionLog> ExecutionLogs { get; }
    FlowExecutionLog CreateLogFor(Flow flow, IVariables flowVariables);
    void CloseLogFor(FlowExecutionLog executionLog, IVariables flowVariables, ExecutionResultType resultType, IFlowTaskData? currentTaskData = null);
}