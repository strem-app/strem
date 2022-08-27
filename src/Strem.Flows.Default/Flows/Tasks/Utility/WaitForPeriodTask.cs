using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;

namespace Strem.Flows.Default.Flows.Tasks.Utility;

public class WaitForPeriodTask : FlowTask<WaitForPeriodTaskData>
{
    public override string Code => WaitForPeriodTaskData.TaskCode;
    public override string Version => WaitForPeriodTaskData.TaskVersion;
    
    public override string Name => "Wait For Period";
    public override string Category => "Utility";
    public override string Description => "Sets a variable for use later within the process";

    public WaitForPeriodTask(ILogger<FlowTask<WaitForPeriodTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    public override bool CanExecute() => true;

    public override async Task<ExecutionResult> Execute(WaitForPeriodTaskData data, IVariables flowVars)
    {
        if (!FlowStringProcessor.TryProcessInt(data.WaitAmount, flowVars, out var intValue))
        {
            Logger.LogWarning($"Unable to process {data.WaitAmount} into a number, verify it is a number or variables exist");
            return ExecutionResult.Failed($"Unable to process integer value from {data.WaitAmount}");
        }
        
        var timespan = data.WaitUnitsType.ToTimeSpan(intValue);
        await Task.Delay(timespan);
        return ExecutionResult.Success();
    }
}