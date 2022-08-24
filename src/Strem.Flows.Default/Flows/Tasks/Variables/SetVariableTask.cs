using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;

namespace Strem.Flows.Default.Flows.Tasks.Variables;

public class SetVariableTask : FlowTask<SetVariableTaskData>
{
    public override string Code => SetVariableTaskData.TaskCode;
    public override string Version => SetVariableTaskData.TaskVersion;
    
    public override string Name => "Set A Variable";
    public override string Category => "Variables";
    public override string Description => "Sets a variable for use later within the process";

    public SetVariableTask(ILogger<FlowTask<SetVariableTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    public override bool CanExecute() => true;

    public override async Task<ExecutionResult> Execute(SetVariableTaskData data, IVariables flowVars)
    {
        var processedName = FlowStringProcessor.Process(data.Name, flowVars);
        var processedContext = FlowStringProcessor.Process(data.Context, flowVars);
        var processedValue = FlowStringProcessor.Process(data.Value, flowVars);

        AppState.SetVariable(flowVars, data.ScopeType, processedName, processedContext, processedValue);
        return ExecutionResult.Success();
    }
}