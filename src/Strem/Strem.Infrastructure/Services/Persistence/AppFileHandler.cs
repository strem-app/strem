using Strem.Core.Flows;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Infrastructure.Services.Persistence.App;
using Strem.Infrastructure.Services.Persistence.Flows;
using Strem.Infrastructure.Services.Persistence.User;

namespace Strem.Infrastructure.Services.Persistence;

public class AppFileHandler : IAppFileHandler
{
    public ISaveUserVariablesPipeline UserVariableSaver { get; }
    public ISaveAppVariablesPipeline AppVariableSaver { get; }
    
    public ILoadUserVariablesPipeline UserVariableLoader { get; }
    public ILoadAppVariablesPipeline AppVariableLoader { get; }
    
    public ILoadFlowStorePipeline FlowStoreLoader { get; }
    public ISaveFlowStorePipeline FlowStoreSaver { get; }

    public AppFileHandler(ISaveUserVariablesPipeline userVariableSaver, ISaveAppVariablesPipeline appVariableSaver, ILoadUserVariablesPipeline userVariableLoader, ILoadAppVariablesPipeline appVariableLoader, ILoadFlowStorePipeline flowStoreLoader, ISaveFlowStorePipeline flowStoreSaver)
    {
        UserVariableSaver = userVariableSaver;
        AppVariableSaver = appVariableSaver;
        UserVariableLoader = userVariableLoader;
        AppVariableLoader = appVariableLoader;
        FlowStoreLoader = flowStoreLoader;
        FlowStoreSaver = flowStoreSaver;
    }

    public async Task CreateAppFilesIfMissing()
    {
        if(!Directory.Exists(PathHelper.AppDirectory))
        { Directory.CreateDirectory(PathHelper.AppDirectory); }
        
        var userFilePath = UserVariableSaver.VariableFilePath;
        if (!File.Exists(userFilePath))
        { await UserVariableSaver.Execute(new Variables()); }

        var appFilePath = AppVariableSaver.VariableFilePath;
        if (!File.Exists(appFilePath))
        { await AppVariableSaver.Execute(new Variables()); }
        
        var flowStoreFilePath = FlowStoreSaver.FlowStoreFilePath;
        if (!File.Exists(flowStoreFilePath))
        { await FlowStoreSaver.Execute(new FlowStore()); }
    }
    
    public async Task<AppState> LoadAppState()
    {
        await CreateAppFilesIfMissing();
        var appVariables = await AppVariableLoader.Execute();
        var userVariables = await UserVariableLoader.Execute();

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
    {
        await AppVariableSaver.Execute(appState.AppVariables);
        await UserVariableSaver.Execute(appState.UserVariables);
    }
}