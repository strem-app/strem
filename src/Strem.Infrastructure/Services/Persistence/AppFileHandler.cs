using System.IO.Compression;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Data.Types;
using Strem.Infrastructure.Extensions;

namespace Strem.Infrastructure.Services.Persistence;

public class AppFileHandler : IAppFileHandler
{
    public IAppVariablesRepository AppVariablesRepository { get; }
    public IUserVariablesRepository UserVariablesRepository { get; }

    public AppFileHandler(IAppVariablesRepository appVariablesRepository, IUserVariablesRepository userVariablesRepository)
    {
        AppVariablesRepository = appVariablesRepository;
        UserVariablesRepository = userVariablesRepository;
    }

    public async Task<AppState> LoadAppState()
    {
        var appVariableData = AppVariablesRepository.GetAll();
        var appVariables = new Variables(new Dictionary<VariableEntry, string>(appVariableData));
        
        var userVariableData = UserVariablesRepository.GetAll();
        var userVariables = new Variables(new Dictionary<VariableEntry, string>(userVariableData));

        return new AppState(userVariables, appVariables, new Variables());
    }
}