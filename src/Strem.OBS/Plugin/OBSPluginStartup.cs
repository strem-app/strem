using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Obs.v5.WebSocket.Reactive;
using Persistity.Encryption;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.OBS.Extensions;

namespace Strem.OBS.Plugin;

public class OBSPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public IEncryptor Encryptor { get; }
    public IObservableOBSWebSocket OBSClient { get; }
    public ILogger<OBSPluginStartup> Logger { get; }

    public OBSPluginStartup(IEventBus eventBus, IAppState appState, ILogger<OBSPluginStartup> logger, IObservableOBSWebSocket obsClient, IEncryptor encryptor)
    {
        EventBus = eventBus;
        AppState = appState;
        Logger = logger;
        OBSClient = obsClient;
        Encryptor = encryptor;
    }

    public string[] RequiredConfigurationKeys => Array.Empty<string>();
    public Task SetupPlugin() => Task.CompletedTask;

    public async Task StartPlugin()
    {
        OBSClient.OnCurrentProgramSceneChanged
            .Subscribe(x => AppState.SetCurrentSceneName(x.SceneName))
            .AddTo(_subs);

        Observable.Interval(TimeSpan.FromSeconds(55))
            .Subscribe(x => DoHeartbeat())
            .AddTo(_subs);

        if (AppState.HasOBSHost())
        {
            await ConnectToOBS();
            await RefreshOBSData();
        }
    }

    public async Task ConnectToOBS()
    {
        var result = await OBSClient.AttemptConnect(AppState, Encryptor);
        if (!result.success)
        { Logger.Error($"Couldnt connect to OBS: {result.message}"); }
        else
        { Logger.Information("Connected To OBS"); }
    }

    public async Task DoHeartbeat()
    {
        if (OBSClient.IsConnected)
        { OBSClient.GetVersion(); }
    }

    public async Task RefreshOBSData()
    {
        if (!OBSClient.IsConnected)
        { await ConnectToOBS(); }

        if (!OBSClient.IsConnected) { return; }
        var sceneName = OBSClient.GetCurrentProgramScene();
        AppState.SetCurrentSceneName(sceneName);
    }

    public void Dispose()
    { _subs?.Dispose(); }
}