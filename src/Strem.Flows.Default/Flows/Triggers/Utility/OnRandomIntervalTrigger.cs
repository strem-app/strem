using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Services.Utils;
using Strem.Flows.Data.Triggers;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Extensions;
using Strem.Flows.Processors;

namespace Strem.Flows.Default.Flows.Triggers.Utility;

public class OnRandomIntervalTrigger : FlowTrigger<OnRandomIntervalTriggerData>
{
    public override string Code => OnRandomIntervalTriggerData.TriggerCode;
    public override string Version => OnRandomIntervalTriggerData.TriggerVersion;

    public override string Name => "On Random Interval";
    public override string Category => "Utility";
    public override string Description => "Triggers the flow randomly the interval is met";

    public IRandomizer Randomizer { get; }
    
    public OnRandomIntervalTrigger(ILogger<FlowTrigger<OnRandomIntervalTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IRandomizer randomizer) : base(logger, flowStringProcessor, appState, eventBus)
    {
        Randomizer = randomizer;
    }

    public override bool CanExecute() => true;
    
    public IVariables SetupVariables() => new Core.Variables.Variables();
    
    public override async Task<IObservable<IVariables>> Execute(OnRandomIntervalTriggerData data)
    {
        var tempFlowVars = new Core.Variables.Variables();

        if (!FlowStringProcessor.TryProcessInt(data.MinIntervalValue, tempFlowVars, out var minIntValue))
        {
            Logger.LogWarning($"Unable to process min value {data.MinIntervalValue} into a number, verify it is a number or variables exist");
            return Observable.Empty<IVariables>();
        }

        if (!FlowStringProcessor.TryProcessInt(data.MaxIntervalValue, tempFlowVars, out var maxIntValue))
        {
            Logger.LogWarning($"Unable to process max value {data.MaxIntervalValue} into a number, verify it is a number or variables exist");
            return Observable.Empty<IVariables>();
        }
        
        var minTimespan = data.MinIntervalUnitsType.ToTimeSpan(minIntValue);
        var maxTimespan = data.MaxIntervalUnitsType.ToTimeSpan(maxIntValue);

        return Observable
            .Generate(0, x => true, x => 0, x => 0, x => Randomizer.PickRandomBetween(minTimespan, maxTimespan))
            .Select(x => SetupVariables());
    }
}