using System.Reactive.Disposables;

namespace Strem.Core.Extensions;

public static class IDisposableExtensions
{
    public static void AddTo(this IDisposable disposable, CompositeDisposable compositeDisposable)
    { compositeDisposable.Add(disposable); }
}