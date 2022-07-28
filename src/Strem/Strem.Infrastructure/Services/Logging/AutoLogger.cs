using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Events;
using Strem.Core.Extensions;
using Strem.Core.State;
using ILogger = Serilog.ILogger;

namespace Strem.Infrastructure.Services.Logging;

public class AutoLogger : IAutoLogger
{
    private CompositeDisposable _subs;
    
    public IAppState AppState { get; }
    public IEventBus EventBus { get; }
    public ILogger Logger { get; }

    public AutoLogger(IAppState appState, IEventBus eventBus, ILogger logger)
    {
        AppState = appState;
        EventBus = eventBus;
        Logger = logger;
    }

    public void EnableAutoLogging()
    {
        _subs = new CompositeDisposable();
        
        AppState.UserVariables.OnVariableChanged
            .Subscribe(x => Logger.Information($"User Variables [{x.Name}|{x.Context}] Changed"))
            .AddTo(_subs);

        AppState.AppVariables.OnVariableChanged
            .Throttle(TimeSpan.FromSeconds(2))
            .Subscribe(x => Logger.Information($"App Variables [{x.Name}|{x.Context}] Changed"))
            .AddTo(_subs);

        EventBus.Receive<ErrorEvent>().Subscribe(x => Logger.Error($"[{x.Source}]: {x.Message}"));
    }

    public void DisableAutoLogging()
    {
        _subs?.Dispose();
        _subs = null;
    }
    
    public void Dispose() => DisableAutoLogging();
}