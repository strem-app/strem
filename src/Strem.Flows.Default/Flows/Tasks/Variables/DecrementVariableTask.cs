using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;

namespace Strem.Flows.Default.Flows.Tasks.Variables;

public class DecrementVariableTask : FlowTask<DecrementVariableTaskData>
{
    public override string Code => DecrementVariableTaskData.TaskCode;
    public override string Version => DecrementVariableTaskData.TaskVersion;
    
    public override string Name => "Decrement A Variable";
    public override string Category => "Variables";
    public override string Description => "Decrements a variable by a given amount";

    public DecrementVariableTask(ILogger<FlowTask<DecrementVariableTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    public override bool CanExecute() => true;

    public override async Task<ExecutionResult> Execute(DecrementVariableTaskData data, IVariables flowVars)
    {
        var processedName = FlowStringProcessor.Process(data.Name, flowVars);
        var processedContext = FlowStringProcessor.Process(data.Context, flowVars);
        var currentValue = AppState.GetVariable(flowVars, processedName, processedContext);
        if(string.IsNullOrEmpty(currentValue)){ currentValue = "0"; }

        int value;
        if(!int.TryParse(currentValue, out value))
        { return ExecutionResult.Failed($"Couldnt parse integer value from ({data.Name}, {data.Context})"); }

        var newValue = value - data.DecrementAmount;
        AppState.SetVariable(flowVars, data.ScopeType, processedName, processedContext, newValue.ToString());
        return ExecutionResult.Success();
    }
}