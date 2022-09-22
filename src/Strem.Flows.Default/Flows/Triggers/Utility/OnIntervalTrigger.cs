using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Flows.Data.Triggers;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Extensions;
using Strem.Flows.Processors;

namespace Strem.Flows.Default.Flows.Triggers.Utility;

public class OnIntervalTrigger : FlowTrigger<OnIntervalTriggerData>
{
    public override string Code => OnIntervalTriggerData.TriggerCode;
    public override string Version => OnIntervalTriggerData.TriggerVersion;

    public override string Name => "On Interval";
    public override string Category => "Utility";
    public override string Description => "Triggers the flow every time the interval is met";

    public OnIntervalTrigger(ILogger<FlowTrigger<OnIntervalTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    public override bool CanExecute() => true;

    public IVariables SetupVariables() => new Core.Variables.Variables();
    
    public override async Task<IObservable<IVariables>> Execute(OnIntervalTriggerData data)
    {
        if (!FlowStringProcessor.TryProcessInt(data.IntervalValue, new Core.Variables.Variables(), out var intValue))
        {
            Logger.LogWarning($"Unable to process {data.IntervalValue} into a number, verify it is a number or variables exist");
            return Observable.Empty<IVariables>();
        }

        var timespan = data.IntervalUnitsType.ToTimeSpan(intValue);

        if (data.StartImmediately)
        { return Observable.Timer(TimeSpan.Zero, timespan).Select(x => SetupVariables()); }

        return Observable.Interval(timespan).Select(x => SetupVariables());
    }
}