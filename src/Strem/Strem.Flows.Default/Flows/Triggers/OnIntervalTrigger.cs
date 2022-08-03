using System.Reactive;
using System.Reactive.Linq;
using Strem.Core.Extensions;
using Strem.Core.Flows.Triggers;
using Strem.Core.Utils;
using Strem.Core.Variables;
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
    
    public override IObservable<IVariables> Execute(OnIntervalTriggerData data)
    {
        var variables = new Variables();
        
        TimeSpan timespan;
        switch (data.IntervalUnits)
        {
            case TimeUnit.Minutes: timespan = TimeSpan.FromMinutes(data.IntervalValue); break;
            case TimeUnit.Hours: timespan = TimeSpan.FromHours(data.IntervalValue); break;
            case TimeUnit.Days: timespan = TimeSpan.FromDays(data.IntervalValue); break;
            default: timespan = TimeSpan.FromSeconds(data.IntervalValue); break;
        }

        if (data.StartImmediately)
        { return Observable.Timer(TimeSpan.Zero, timespan).Select(x => variables); }

        return Observable.Interval(timespan).Select(x => variables);
    }
}