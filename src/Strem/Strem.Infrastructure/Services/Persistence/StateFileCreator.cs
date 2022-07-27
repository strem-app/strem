using Strem.Core.State;
using Strem.Infrastructure.Services.Persistence.App;
using Strem.Infrastructure.Services.Persistence.User;

namespace Strem.Infrastructure.Services.Persistence;

public class StateFileCreator
{
    public ISaveUserVariablesPipeline UserVariableSaver { get; }
    public ISaveAppVariablesPipeline AppVariableSaver { get; }

    public StateFileCreator(ISaveUserVariablesPipeline userVariableSaver, ISaveAppVariablesPipeline appVariableSaver)
    {
        UserVariableSaver = userVariableSaver;
        AppVariableSaver = appVariableSaver;
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
}