using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Triggers;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Utils;
using Strem.Core.Variables;
using Strem.Flows.Default.Flows.Tasks;
using Strem.Flows.Default.Flows.Triggers.Data;

namespace Strem.Flows.Default.Flows.Triggers;

public class OnIntervalTrigger : FlowTrigger<OnIntervalTriggerData>
{
    public static readonly string TriggerCode = "on-interval";
    public static readonly string TriggerVersion = "1.0.0";
    public override string Code => TriggerCode;
    public override string Version => TriggerVersion;

    public override string Name => "On Interval";
    public override string Description => "Triggers the flow every time the interval is met";

    public OnIntervalTrigger(ILogger<IFlowTrigger> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) 
        : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    public override IObservable<IVariables> Execute(OnIntervalTriggerData data)
    {
        var variables = new Variables();

        if (!FlowStringProcessor.TryProcessInt(data.IntervalValue, variables, out var intValue))
        {
            Logger.LogWarning($"Unable to process {data.IntervalValue} into a number, verify it is a number or variables exist");
            return Observable.Empty<IVariables>();
        }

        var timespan = data.IntervalUnits.ToTimeSpan(intValue);

        if (data.StartImmediately)
        { return Observable.Timer(TimeSpan.Zero, timespan).Select(x => variables); }

        return Observable.Interval(timespan).Select(x => variables);
    }
}