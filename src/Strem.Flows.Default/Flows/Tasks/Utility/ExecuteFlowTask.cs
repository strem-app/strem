using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;

namespace Strem.Flows.Default.Flows.Tasks.Utility;

public class ExecuteFlowTask : FlowTask<ExecuteFlowTaskData>
{
    public override string Code => ExecuteFlowTaskData.TaskCode;
    public override string Version => ExecuteFlowTaskData.TaskVersion;
    
    public override string Name => "Execute Flow";
    public override string Category => "Utility";
    public override string Description => "Executes an existing flow";

    public IFlowExecutor FlowExecutor { get; }

    public ExecuteFlowTask(ILogger<FlowTask<ExecuteFlowTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IFlowExecutor flowExecutor) : base(logger, flowStringProcessor, appState, eventBus)
    {
        FlowExecutor = flowExecutor;
    }

    public override bool CanExecute() => true;

    public override async Task<ExecutionResult> Execute(ExecuteFlowTaskData data, IVariables flowVars)
    {
        if (data.WaitForCompletion)
        { await FlowExecutor.ExecuteFlow(data.FlowId, flowVars); }
        else
        { FlowExecutor.ExecuteFlow(data.FlowId, flowVars); }

        return ExecutionResult.Success();
    }
}