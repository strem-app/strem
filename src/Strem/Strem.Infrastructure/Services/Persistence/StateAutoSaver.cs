using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Infrastructure.Services.Persistence.App;
using Strem.Infrastructure.Services.Persistence.User;

namespace Strem.Infrastructure.Services.Persistence;

public class StateAutoSaver : IStateAutoSaver
{
    private CompositeDisposable _subs;
    
    public IAppState AppState { get; }
    public ISaveUserVariablesPipeline UserVariableSaver { get; }
    public ISaveAppVariablesPipeline AppVariableSaver { get; }

    public StateAutoSaver(IAppState appState, ISaveUserVariablesPipeline userVariableSaver, ISaveAppVariablesPipeline appVariableSaver)
    {
        AppState = appState;
        UserVariableSaver = userVariableSaver;
        AppVariableSaver = appVariableSaver;
    }

    public void EnableAutoSaving()
    {
        _subs = new CompositeDisposable();
        
        AppState.UserVariables.OnVariableChanged
            .Throttle(TimeSpan.FromSeconds(2))
            .Subscribe(_ => UserVariableSaver.Execute(AppState.UserVariables))
            .AddTo(_subs);

        AppState.AppVariables.OnVariableChanged
            .Throttle(TimeSpan.FromSeconds(2))
            .Subscribe(_ => AppVariableSaver.Execute(AppState.AppVariables))
            .AddTo(_subs);
    }

    public void DisableAutoSaving()
    {
        _subs?.Dispose();
        _subs = null;
    }
    
    public void Dispose() => DisableAutoSaving();
}