using System.IO.Compression;
using Strem.Core.Flows;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Infrastructure.Extensions;
using Strem.Infrastructure.Services.Persistence.App;
using Strem.Infrastructure.Services.Persistence.Flows;
using Strem.Infrastructure.Services.Persistence.User;

namespace Strem.Infrastructure.Services.Persistence;

public class AppFileHandler : IAppFileHandler
{
    public ISaveUserDataPipeline UserDataSaver { get; }
    public ISaveAppDataPipeline AppDataSaver { get; }
    
    public ILoadUserDataPipeline UserDataLoader { get; }
    public ILoadAppDataPipeline AppDataLoader { get; }
    
    public ILoadFlowStorePipeline FlowStoreLoader { get; }
    public ISaveFlowStorePipeline FlowStoreSaver { get; }


    public AppFileHandler(ISaveUserDataPipeline userDataSaver, ISaveAppDataPipeline appDataSaver, ILoadUserDataPipeline userDataLoader, ILoadAppDataPipeline appDataLoader, ILoadFlowStorePipeline flowStoreLoader, ISaveFlowStorePipeline flowStoreSaver)
    {
        UserDataSaver = userDataSaver;
        AppDataSaver = appDataSaver;
        UserDataLoader = userDataLoader;
        AppDataLoader = appDataLoader;
        FlowStoreLoader = flowStoreLoader;
        FlowStoreSaver = flowStoreSaver;
    }

    public async Task CreateAppFilesIfMissing()
    {
        if (!Directory.Exists(PathHelper.StremDataDirectory))
        { Directory.CreateDirectory(PathHelper.StremDataDirectory); }
        
        var userFilePath = UserDataSaver.DataFilePath;
        if (!File.Exists(userFilePath))
        { await UserDataSaver.Execute(new Variables()); }

        var appFilePath = AppDataSaver.DataFilePath;
        if (!File.Exists(appFilePath))
        { await AppDataSaver.Execute(new Variables()); }
        
        var flowStoreFilePath = FlowStoreSaver.DataFilePath;
        if (!File.Exists(flowStoreFilePath))
        { await FlowStoreSaver.Execute(new FlowStore()); }
    }
    
    public async Task<AppState> LoadAppState()
    {
        await CreateAppFilesIfMissing();
        var appVariables = await AppDataLoader.Execute();
        var userVariables = await UserDataLoader.Execute();

        return new AppState(
            userVariables ?? new Variables(),
            appVariables ?? new Variables(),
            new Variables()
        );
    }

    public async Task<FlowStore> LoadFlowStore()
    {
        await CreateAppFilesIfMissing();
        var flowStore = await FlowStoreLoader.Execute();
        return flowStore ?? new FlowStore();
    }

    public async Task SaveAppState(IAppState appState)
    { await AppDataSaver.Execute(appState.AppVariables); }
    
    public async Task SaveUserState(IAppState appState)
    { await UserDataSaver.Execute(appState.UserVariables); }

    public async Task SaveFlowStore(IFlowStore flowStore)
    { await FlowStoreSaver.Execute(flowStore); }
    
    public async Task BackupFiles()
    {
        var backupDir = $"{PathHelper.StremDataDirectory}/backups";
        if (!Directory.Exists(backupDir))
        { Directory.CreateDirectory(backupDir); }
        
        var dateFormat = DateTime.Now.ToString("yyMMdd");
        var backupFile = $"{backupDir}/data-backup-{dateFormat}.zip";
        if (File.Exists(backupFile)) { return; }
        
        using var zip = ZipFile.Open(backupFile, ZipArchiveMode.Create);
        { zip.CreateEntryFromGlob(PathHelper.StremDataDirectory, "*.dat"); }
    }
}