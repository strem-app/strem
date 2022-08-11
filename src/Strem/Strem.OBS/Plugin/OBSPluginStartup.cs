using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using OBSWebsocketDotNet.Types;
using Persistity.Encryption;
using Persistity.Extensions;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.OBS.Extensions;
using Strem.OBS.Services.Client;
using Strem.OBS.Variables;

namespace Strem.OBS.Plugin;

public class OBSPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public IEncryptor Encryptor { get; }
    public IObservableOBSClient OBSClient { get; }
    public ILogger<OBSPluginStartup> Logger { get; }

    public OBSPluginStartup(IEventBus eventBus, IAppState appState, ILogger<OBSPluginStartup> logger, IObservableOBSClient obsClient, IEncryptor encryptor)
    {
        EventBus = eventBus;
        AppState = appState;
        Logger = logger;
        OBSClient = obsClient;
        Encryptor = encryptor;
    }

    public async Task StartPlugin()
    {
        OBSClient.OnUnhandledEvent
            .Subscribe(x => Logger.Error($"Unhandled Event in OBS: {x}"))
            .AddTo(_subs);
        
        OBSClient.OnOBSError
            .Subscribe(x => Logger.Error($"Unhandled Error in OBS: {x}"))
            .AddTo(_subs);
        
        if (AppState.HasOBSHost())
        { await ConnectToOBS(); }
        
        Observable.Timer(TimeSpan.Zero, TimeSpan.FromMinutes(OBSPluginSettings.RefreshScenePeriodInSeconds))
            .Subscribe(x => OBSClient.PopulateActiveSceneTransientData(AppState))
            .AddTo(_subs);

        OBSClient.OnSceneChanged
            .Subscribe(x => AppState.UpdateActiveSceneTransientData(x.NewSceneName, x.SceneItems))
            .AddTo(_subs);
    }

    public async Task ConnectToOBS()
    {
        var host = AppState.AppVariables.Get(OBSVars.Host);
        var port = AppState.AppVariables.Get(OBSVars.Port);
        var password = string.Empty;

        if (AppState.AppVariables.Has(OBSVars.Password))
        {
            var encryptedPassword = AppState.AppVariables.Get(OBSVars.Password);
            password = Encryptor.Decrypt(encryptedPassword);
        }

        var result = await OBSClient.AttemptConnect(host, port, password);
        if (!result.success)
        { Logger.Error($"Couldnt connect to OBS: {result.message}"); }
        else
        { Logger.Information($"Connected to OBS Server at {host}"); }
    }

    public void Dispose()
    { _subs?.Dispose(); }
}