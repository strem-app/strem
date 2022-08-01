using System.Reactive;
using System.Reactive.Linq;

namespace Strem.Core.Extensions;

public static class IObservableExtensions
{
    public static IObservable<Unit> ToUnit<T>(this IObservable<T> observable)
    { return observable.Select(x => Unit.Default); }
}