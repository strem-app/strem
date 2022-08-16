using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;

namespace Strem.Flows.Default.Flows.Tasks.Variables;

public class IncrementVariableTask : FlowTask<IncrementVariableTaskData>
{
    public override string Code => IncrementVariableTaskData.TaskCode;
    public override string Version => IncrementVariableTaskData.TaskVersion;
    
    public override string Name => "Increment A Variable";
    public override string Category => "Variables";
    public override string Description => "Increments an existing variables value by a given amount";

    public IncrementVariableTask(ILogger<FlowTask<IncrementVariableTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    public override bool CanExecute() => true;

    public override async Task<bool> Execute(IncrementVariableTaskData data, IVariables flowVars)
    {
        var processedName = FlowStringProcessor.Process(data.Name, flowVars);
        var processedContext = FlowStringProcessor.Process(data.Context, flowVars);
        var currentValue = AppState.GetVariable(flowVars, processedName, processedContext);
        if(string.IsNullOrEmpty(currentValue)){ return false; }

        int value;
        if(!int.TryParse(currentValue, out value))
        { return false; }

        var newValue = value + data.IncrementAmount;
        AppState.SetVariable(flowVars, data.ScopeType, processedName, processedContext);
        return true;
    }
}