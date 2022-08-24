using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Events;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Infrastructure.Services.Persistence;

namespace Strem.Infrastructure.Plugin;

public class InfrastructurePluginStartup : IPluginStartup, IDisposable
{
    private readonly CompositeDisposable _subs = new();
    
    public IAppFileHandler AppFileHandler { get; }
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public IFlowStore FlowStore { get; }
    public ILogger<InfrastructurePluginStartup> Logger { get; }

    public InfrastructurePluginStartup(IAppFileHandler appFileHandler, IEventBus eventBus, IAppState appState, IFlowStore flowStore, ILogger<InfrastructurePluginStartup> logger)
    {
        AppFileHandler = appFileHandler;
        EventBus = eventBus;
        AppState = appState;
        FlowStore = flowStore;
        Logger = logger;
    }

    public async Task StartPlugin()
    {
        AppState.UserVariables.OnVariableChanged
            .Throttle(TimeSpan.FromSeconds(5))
            .Subscribe(_ =>
            {
                Logger.Information("Saving User Vars");
                AppFileHandler.SaveUserState(AppState);
            })
            .AddTo(_subs);

        AppState.AppVariables.OnVariableChanged
            .Throttle(TimeSpan.FromSeconds(5))
            .Subscribe(_ =>
            {
                Logger.Information("Saving App Vars");
                AppFileHandler.SaveAppState(AppState);
            })
            .AddTo(_subs);
        
        AppState.UserVariables.OnVariableChanged
            .Subscribe(x => Logger.Information($"User Variables [{x.Name}|{x.Context}] Changed"))
            .AddTo(_subs);

        AppState.AppVariables.OnVariableChanged
            .Subscribe(x => Logger.Information($"App Variables [{x.Name}|{x.Context}] Changed"))
            .AddTo(_subs);

        EventBus.Receive<ErrorEvent>().Subscribe(x => Logger.Error($"[{x.Source}]: {x.Message}"));
        
        Observable.Interval(TimeSpan.FromMinutes(5))
            .Subscribe(x =>
            {
                Logger.Information("Saving Flows");
                AppFileHandler.SaveFlowStore(FlowStore);
            })
            .AddTo(_subs);

        CheckIfBackupIsNeeded();
    }

    public void CheckIfBackupIsNeeded()
    {
        var lastBackupDate = DateTime.MinValue;
        if (AppState.AppVariables.Has(UIVariables.LastBackupDate))
        { DateTime.TryParse(AppState.AppVariables.Get(UIVariables.LastBackupDate), out lastBackupDate); }

        var backupIntervalInDays = 7;
        if (AppState.AppVariables.Has(UIVariables.BackupFrequencyInDays))
        {
            var requestedFrequency = AppState.AppVariables.Get<int>(UIVariables.BackupFrequencyInDays);
            if(requestedFrequency > 0)
            { backupIntervalInDays = requestedFrequency; }
        }

        var timeSinceLastBackup = DateTime.Now - lastBackupDate ;
        if(timeSinceLastBackup.TotalDays < backupIntervalInDays) { return; }

        Logger.Information("Making a backup of app related data");
        AppFileHandler.BackupFiles();
        AppState.AppVariables.Set(UIVariables.LastBackupDate, DateTime.Now.ToString("u"));
        Logger.Information($"Finished making a backup of app related data, will try again in {backupIntervalInDays} days");
    }

    public void Dispose()
    {
        _subs?.Dispose();
    }
}