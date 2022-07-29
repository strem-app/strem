using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Events;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Infrastructure.Services.Persistence.App;
using Strem.Infrastructure.Services.Persistence.User;
using ILogger = Serilog.ILogger;

namespace Strem.Infrastructure.Plugin;

public class InfrastructurePluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs;
    
    public ISaveUserVariablesPipeline UserVariableSaver { get; }
    public ISaveAppVariablesPipeline AppVariableSaver { get; }
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public ILogger Logger { get; }

    public InfrastructurePluginStartup(ISaveUserVariablesPipeline userVariableSaver, ISaveAppVariablesPipeline appVariableSaver, IEventBus eventBus, IAppState appState, ILogger logger)
    {
        UserVariableSaver = userVariableSaver;
        AppVariableSaver = appVariableSaver;
        EventBus = eventBus;
        AppState = appState;
        Logger = logger;
    }

    public async Task StartPlugin()
    {
        _subs = new CompositeDisposable();
        AppState.UserVariables.OnVariableChanged
            .Throttle(TimeSpan.FromSeconds(5))
            .Subscribe(_ => UserVariableSaver.Execute(AppState.UserVariables))
            .AddTo(_subs);

        AppState.AppVariables.OnVariableChanged
            .Throttle(TimeSpan.FromSeconds(5))
            .Subscribe(_ => AppVariableSaver.Execute(AppState.AppVariables))
            .AddTo(_subs);
        
        AppState.UserVariables.OnVariableChanged
            .Subscribe(x => Logger.Information($"User Variables [{x.Name}|{x.Context}] Changed"))
            .AddTo(_subs);

        AppState.AppVariables.OnVariableChanged
            .Subscribe(x => Logger.Information($"App Variables [{x.Name}|{x.Context}] Changed"))
            .AddTo(_subs);

        EventBus.Receive<ErrorEvent>().Subscribe(x => Logger.Error($"[{x.Source}]: {x.Message}"));
    }

    public void Dispose()
    {
        _subs?.Dispose();
    }
}