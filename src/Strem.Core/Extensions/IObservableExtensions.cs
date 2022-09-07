using System.Reactive;
using System.Reactive.Linq;

namespace Strem.Core.Extensions;

public static class IObservableExtensions
{
    public static IObservable<Unit> ToUnit<T>(this IObservable<T> observable)
    { return observable.Select(x => Unit.Default); }
    
    public static IObservable<T> ThrottledByKey<T>(this IObservable<T> observable, Func<T, object> keySelector, TimeSpan throttlePeriod)
    {
        return observable
            .GroupByUntil(keySelector, x => Observable.Timer(throttlePeriod))
            .SelectMany(x => x.LastAsync());
    }
    
    public static IObservable<T> ThrottledByKey<T, TK>(this IObservable<T> observable, Func<T, TK> keySelector, TimeSpan throttlePeriod)
        where T : struct
    {
        return observable
            .GroupByUntil(keySelector, x => Observable.Timer(throttlePeriod))
            .SelectMany(x => x.LastAsync());
    }
}