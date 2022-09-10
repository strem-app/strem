using System.Reactive.Linq;
using Strem.Core.Events.Bus;

namespace Strem.Core.Extensions;

public static class IEventBusExtensions
{
    public static IObservable<TBase> ReceiveAs<TBase, T1, T2>(this IEventBus eventBus)
        where T1 : TBase
        where T2 : TBase
    {
        return eventBus
            .Receive<T1>().Select(x => (TBase)x)
            .Merge(eventBus.Receive<T2>().Select(x => (TBase)x));
    }
    
    public static IObservable<TBase> ReceiveAs<TBase, T1, T2, T3>(this IEventBus eventBus)
        where T1 : TBase
        where T2 : TBase
        where T3 : TBase
    {
        return eventBus
            .Receive<T1>().Select(x => (TBase)x)
            .Merge(eventBus.Receive<T2>().Select(x => (TBase)x))
            .Merge(eventBus.Receive<T3>().Select(x => (TBase)x));
    }
    
    public static IObservable<TBase> ReceiveAs<TBase, T1, T2, T3, T4>(this IEventBus eventBus)
        where T1 : TBase
        where T2 : TBase
        where T3 : TBase
        where T4 : TBase
    {
        return eventBus
            .Receive<T1>().Select(x => (TBase)x)
            .Merge(eventBus.Receive<T2>().Select(x => (TBase)x))
            .Merge(eventBus.Receive<T3>().Select(x => (TBase)x))
            .Merge(eventBus.Receive<T4>().Select(x => (TBase)x));
    }    
    
    public static IObservable<TBase> ReceiveAs<TBase, T1, T2, T3, T4, T5>(this IEventBus eventBus)
        where T1 : TBase
        where T2 : TBase
        where T3 : TBase
        where T4 : TBase
        where T5 : TBase
    {
        return eventBus
            .Receive<T1>().Select(x => (TBase)x)
            .Merge(eventBus.Receive<T2>().Select(x => (TBase)x))
            .Merge(eventBus.Receive<T3>().Select(x => (TBase)x))
            .Merge(eventBus.Receive<T4>().Select(x => (TBase)x))
            .Merge(eventBus.Receive<T5>().Select(x => (TBase)x));
    }
}