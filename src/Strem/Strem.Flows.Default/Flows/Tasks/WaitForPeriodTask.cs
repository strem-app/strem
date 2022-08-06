using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Default.Flows.Tasks.Data;

namespace Strem.Flows.Default.Flows.Tasks;

public class WaitForPeriodTask : FlowTask<WaitForPeriodTaskData>
{
    public override string Code => WaitForPeriodTaskData.TaskCode;
    public override string Version => WaitForPeriodTaskData.TaskVersion;
    
    public override string Name => "Wait For Period";
    public override string Description => "Sets a variable for use later within the process";

    public WaitForPeriodTask(ILogger<FlowTask<WaitForPeriodTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    public override bool CanExecute() => true;

    public override async Task Execute(WaitForPeriodTaskData data, IVariables flowVars)
    {
        if (!FlowStringProcessor.TryProcessInt(data.WaitAmount, flowVars, out var intValue))
        {
            Logger.LogWarning($"Unable to process {data.WaitAmount} into a number, verify it is a number or variables exist");
            return;
        }
        
        var timespan = data.WaitUnits.ToTimeSpan(intValue);
        await Task.Delay(timespan);
    }
}