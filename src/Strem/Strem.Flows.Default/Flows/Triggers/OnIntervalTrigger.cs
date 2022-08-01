using System.Reactive;
using System.Reactive.Linq;
using Strem.Core.Extensions;
using Strem.Core.Flows.Triggers;
using Strem.Core.Utils;
using Strem.Core.Variables;
using Strem.Flows.Default.Flows.Triggers.Data;

namespace Strem.Flows.Default.Flows.Triggers;

public class OnIntervalTrigger : IFlowTrigger<OnIntervalTriggerData>
{
    public static readonly string TriggerCode = "on-interval";
    public static readonly string TriggerVersion = "1.0.0";
    public string Code => TriggerCode;
    public string Version => TriggerVersion;

    public string Name => "On Interval";
    public string Description => "Triggers the flow every time the interval is met";
    
    public IObservable<Unit> Execute(OnIntervalTriggerData data, IVariables flowVars)
    {
        TimeSpan timespan;
        switch (data.IntervalUnits)
        {
            case TimeUnit.Minutes: timespan = TimeSpan.FromMinutes(data.IntervalValue); break;
            case TimeUnit.Hours: timespan = TimeSpan.FromHours(data.IntervalValue); break;
            case TimeUnit.Days: timespan = TimeSpan.FromDays(data.IntervalValue); break;
            default: timespan = TimeSpan.FromSeconds(data.IntervalValue); break;
        }

        if (data.StartImmediately)
        { return Observable.Timer(TimeSpan.Zero, timespan).ToUnit(); }

        return Observable.Interval(timespan).ToUnit();
    }
}