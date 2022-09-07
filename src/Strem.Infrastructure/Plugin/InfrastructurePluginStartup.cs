using System.IO.Compression;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Events;
using Strem.Core.Events.Bus;
using Strem.Core.Events.Flows;
using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Data.Extensions;
using Strem.Data.Types;
using Strem.Infrastructure.Extensions;
using Strem.Infrastructure.Services.Persistence;

namespace Strem.Infrastructure.Plugin;

public class InfrastructurePluginStartup : IPluginStartup, IDisposable
{
    private readonly CompositeDisposable _subs = new();
    
    public IAppState AppState { get; }
    public IEventBus EventBus { get; }
    public IFlowStore FlowStore { get; }
    public IFlowRepository FlowRepository { get; set; }
    public IAppVariablesRepository AppVariablesRepository { get; }
    public IUserVariablesRepository UserVariablesRepository { get; }
    public ILogger<InfrastructurePluginStartup> Logger { get; }

    public InfrastructurePluginStartup(IAppState appState, IEventBus eventBus, IFlowStore flowStore, IFlowRepository flowRepository, IAppVariablesRepository appVariablesRepository, IUserVariablesRepository userVariablesRepository, ILogger<InfrastructurePluginStartup> logger)
    {
        AppState = appState;
        EventBus = eventBus;
        FlowStore = flowStore;
        FlowRepository = flowRepository;
        AppVariablesRepository = appVariablesRepository;
        UserVariablesRepository = userVariablesRepository;
        Logger = logger;
    }

    public async Task StartPlugin()
    {
        AppState.UserVariables.OnVariableChanged
            .Subscribe(x =>
            {
                if (AppState.UserVariables.Has(x.Key))
                { UserVariablesRepository.Upsert(x.Key, x); }
                else
                { UserVariablesRepository.Delete(x.Key); }
            })
            .AddTo(_subs);

        AppState.AppVariables.OnVariableChanged
            .Subscribe(x =>
            {
                if (AppState.AppVariables.Has(x.Key))
                { AppVariablesRepository.Upsert(x.Key, x); }
                else
                { AppVariablesRepository.Delete(x.Key); }
            })
            .AddTo(_subs);
        
        AppState.UserVariables.OnVariableChanged
            .Subscribe(x => Logger.Information($"User Variables [{x.Key.Name}|{x.Key.Context}] Changed"))
            .AddTo(_subs);

        AppState.AppVariables.OnVariableChanged
            .Subscribe(x => Logger.Information($"App Variables [{x.Key.Name}|{x.Key.Context}] Changed"))
            .AddTo(_subs);

        EventBus.Receive<ErrorEvent>().Subscribe(x => Logger.Error($"[{x.Source}]: {x.Message}"));
        
        EventBus
            .Receive<FlowTaskChangedEvent>().Select(x => x.FlowId)
            .Merge(EventBus.Receive<FlowTriggerChangedEvent>().Select(x => x.FlowId))
            .Merge(EventBus.Receive<FlowDetailsChangedEvent>().Select(x => x.FlowId))
            .ThrottledByKey(x => x, TimeSpan.FromSeconds(2))
            .Select(x => FlowStore.Get(x))
            .Subscribe(x =>
            {
                if(x == null){ return; }
                FlowRepository.Update(x.Id, x);
            })
            .AddTo(_subs);
        
        EventBus.Receive<FlowRemovedEvent>()
            .Select(x => FlowStore.Get(x.FlowId))
            .Subscribe(x =>
            {
                if(x == null){ return; }
                FlowRepository.Delete(x.Id);
            });
        
        EventBus.Receive<FlowAddedEvent>()
            .Select(x => FlowStore.Get(x.FlowId))
            .Subscribe(x =>
            {
                if(x == null){ return; }
                FlowRepository.Create(x.Id, x);
            });

        SetDefaultSettingsIfNotSet();
        CheckIfBackupIsNeeded();
    }
    
    public void SetDefaultSettingsIfNotSet()
    {
        if(!AppState.AppVariables.Has(UIVariables.ShowHelpersVariable))
        { AppState.AppVariables.Set(UIVariables.ShowHelpersVariable, true); }
        
        if(!AppState.AppVariables.Has(UIVariables.ZoomVariable))
        { AppState.AppVariables.Set(UIVariables.ZoomVariable, 100); }
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
        BackupFiles();
        AppState.AppVariables.Set(UIVariables.LastBackupDate, DateTime.Now.ToString("u"));
        Logger.Information($"Finished making a backup of app related data, will try again in {backupIntervalInDays} days");
    }

    public void BackupFiles()
    {
        // TODO: Need to now figure out how to backup litedb as you cant copy while its running
        var backupDir = $"{PathHelper.StremDataDirectory}/backups";
        if (!Directory.Exists(backupDir))
        { Directory.CreateDirectory(backupDir); }
        
        var dateFormat = DateTime.Now.ToString("yyMMdd");
        var backupFile = $"{backupDir}/data-backup-{dateFormat}.zip";
        if (File.Exists(backupFile)) { return; }
        
        using var zip = ZipFile.Open(backupFile, ZipArchiveMode.Create);
        { zip.CreateEntryFromGlob(PathHelper.StremDataDirectory, "*.dat"); }
    }
    
    public void Dispose()
    {
        _subs?.Dispose();
    }
}