using System.IO.Compression;
using Strem.Core.Flows;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Data.Types;
using Strem.Infrastructure.Extensions;

namespace Strem.Infrastructure.Services.Persistence;

public class AppFileHandler : IAppFileHandler
{
    public IFlowRepository FlowRepository { get; }
    public IAppVariablesRepository AppVariablesRepository { get; }
    public IUserVariablesRepository UserVariablesRepository { get; }

    public AppFileHandler(IFlowRepository flowRepository, IAppVariablesRepository appVariablesRepository, IUserVariablesRepository userVariablesRepository)
    {
        FlowRepository = flowRepository;
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

    public async Task<FlowStore> LoadFlowStore()
    {
        var flows = FlowRepository.GetAll();
        return new FlowStore(flows);
    }
}