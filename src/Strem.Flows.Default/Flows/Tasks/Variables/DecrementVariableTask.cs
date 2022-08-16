﻿using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;

namespace Strem.Flows.Default.Flows.Tasks.Variables;

public class DecrementVariableTask : FlowTask<DecrementVariableTaskData>
{
    public override string Code => DecrementVariableTaskData.TaskCode;
    public override string Version => DecrementVariableTaskData.TaskVersion;
    
    public override string Name => "Decrement A Variable";
    public override string Category => "Variables";
    public override string Description => "Decrements a variables value by a given amount";

    public DecrementVariableTask(ILogger<FlowTask<DecrementVariableTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    public override bool CanExecute() => true;

    public override async Task<bool> Execute(DecrementVariableTaskData data, IVariables flowVars)
    {
        var processedName = FlowStringProcessor.Process(data.Name, flowVars);
        var processedContext = FlowStringProcessor.Process(data.Context, flowVars);
        var currentValue = AppState.GetVariable(flowVars, processedName, processedContext);
        if(string.IsNullOrEmpty(currentValue)){ return false; }

        int value;
        if(!int.TryParse(currentValue, out value))
        { return false; }

        var newValue = value - data.DecrementAmount;
        AppState.SetVariable(flowVars, data.ScopeType, processedName, processedContext, newValue.ToString());
        return true;
    }
}