using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Events;
using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Infrastructure.Services.Persistence.App;
using Strem.Infrastructure.Services.Persistence.Flows;
using Strem.Infrastructure.Services.Persistence.User;

namespace Strem.Infrastructure.Plugin;

public class InfrastructurePluginStartup : IPluginStartup, IDisposable
{
    private readonly CompositeDisposable _subs = new();
    
    public ISaveUserVariablesPipeline UserVariableSaver { get; }
    public ISaveAppVariablesPipeline AppVariableSaver { get; }
    public ISaveFlowStorePipeline FlowStoreSaver { get; }
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public IFlowStore FlowStore { get; }
    public ILogger<InfrastructurePluginStartup> Logger { get; }

    public InfrastructurePluginStartup(ISaveUserVariablesPipeline userVariableSaver, ISaveAppVariablesPipeline appVariableSaver, ISaveFlowStorePipeline flowStoreSaver, IEventBus eventBus, IAppState appState, IFlowStore flowStore, ILogger<InfrastructurePluginStartup> logger)
    {
        UserVariableSaver = userVariableSaver;
        AppVariableSaver = appVariableSaver;
        FlowStoreSaver = flowStoreSaver;
        EventBus = eventBus;
        AppState = appState;
        FlowStore = flowStore;
        Logger = logger;
    }

    public async Task StartPlugin()
    {
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
        
        
        Observable.Interval(TimeSpan.FromMinutes(2))
            .Subscribe(x => FlowStoreSaver.Execute(FlowStore))
            .AddTo(_subs);
    }

    public void Dispose()
    {
        _subs?.Dispose();
    }
}