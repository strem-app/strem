using Strem.Core.Extensions;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;

namespace Strem.Core.Flows.Executors.Logging;

public class FlowExecutionLogger : IFlowExecutionLogger
{
    public List<FlowExecutionLog> Logs { get; } = new();
    public IReadOnlyCollection<FlowExecutionLog> ExecutionLogs => Logs;
    
    public IAppState AppState { get; }

    public FlowExecutionLogger(IAppState appState)
    {
        AppState = appState;
    }

    public FlowExecutionLog CreateLogFor(Flow flow, IVariables flowVariables)
    {
        var executionLog = new FlowExecutionLog
        {
            FlowId = flow.Id,
            FlowName = flow.Name,
            StartTime = DateTime.Now,
            StartVariables = flowVariables.Combine(AppState.UserVariables, AppState.TransientVariables)
        };
        Logs.Add(executionLog);
        return executionLog;
    }

    public void CloseLogFor(FlowExecutionLog executionLog, IVariables flowVariables, ExecutionResultType resultType, IFlowTaskData? currentTaskData = null)
    {
        executionLog.EndTime = DateTime.Now;
        executionLog.EndVariables = flowVariables.Combine(AppState.UserVariables, AppState.TransientVariables);
        executionLog.ExecutionResultType = resultType;
        
        if(currentTaskData != null)
        { executionLog.ElementExecutionSummary.Add($"{currentTaskData.Code} - {resultType}"); }
    }
}