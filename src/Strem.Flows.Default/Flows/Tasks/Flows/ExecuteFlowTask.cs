﻿using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Executors;
using Strem.Flows.Processors;

namespace Strem.Flows.Default.Flows.Tasks.Flows;

public class ExecuteFlowTask : FlowTask<ExecuteFlowTaskData>
{
    public override string Code => ExecuteFlowTaskData.TaskCode;
    public override string Version => ExecuteFlowTaskData.TaskVersion;
    
    public override string Name => "Execute Flow";
    public override string Category => "Flows";
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