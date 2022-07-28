using Strem.Core.State;
using Strem.Infrastructure.Services.Persistence.App;
using Strem.Infrastructure.Services.Persistence.User;

namespace Strem.Infrastructure.Services.Persistence;

public class StateFileHandler : IStateFileHandler
{
    public ISaveUserVariablesPipeline UserVariableSaver { get; }
    public ISaveAppVariablesPipeline AppVariableSaver { get; }
    
    public ILoadUserVariablesPipeline UserVariableLoader { get; }
    public ILoadAppVariablesPipeline AppVariableLoader { get; }

    public StateFileHandler(ISaveUserVariablesPipeline userVariableSaver, ISaveAppVariablesPipeline appVariableSaver, ILoadUserVariablesPipeline userVariableLoader, ILoadAppVariablesPipeline appVariableLoader)
    {
        UserVariableSaver = userVariableSaver;
        AppVariableSaver = appVariableSaver;
        UserVariableLoader = userVariableLoader;
        AppVariableLoader = appVariableLoader;
    }

    public async Task CreateStateFilesIfMissing()
    {
        if(!Directory.Exists(PathHelper.AppDirectory))
        { Directory.CreateDirectory(PathHelper.AppDirectory); }
        
        var userFilePath = UserVariableSaver.VariableFilePath;
        if (!File.Exists(userFilePath))
        { await UserVariableSaver.Execute(new Variables()); }

        var appFilePath = AppVariableSaver.VariableFilePath;
        if (!File.Exists(appFilePath))
        { await AppVariableSaver.Execute(new Variables()); }
    }
    
    public async Task<AppState> LoadAppState()
    {
        await CreateStateFilesIfMissing();
        var appVariables = await AppVariableLoader.Execute();
        var userVariables = await UserVariableLoader.Execute();

        return new AppState(
            userVariables ?? new Variables(),
            appVariables ?? new Variables(),
            new Variables()
        );
    }
    
    public async Task SaveAppState(IAppState appState)
    {
        await AppVariableSaver.Execute(appState.AppVariables);
        await UserVariableSaver.Execute(appState.UserVariables);
    }
}