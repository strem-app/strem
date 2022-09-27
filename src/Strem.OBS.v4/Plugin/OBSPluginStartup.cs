using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Obs.v4.WebSocket.Reactive;
using Persistity.Encryption;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.OBS.v4.Extensions;

namespace Strem.OBS.v4.Plugin;

public class OBSPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public IEncryptor Encryptor { get; }
    public IObservableOBSWebSocket OBSClient { get; }
    public ILogger<OBSPluginStartup> Logger { get; }
    
    public string[] RequiredConfigurationKeys { get; } = Array.Empty<string>();

    public OBSPluginStartup(IEventBus eventBus, IAppState appState, ILogger<OBSPluginStartup> logger, IObservableOBSWebSocket obsClient, IEncryptor encryptor)
    {
        EventBus = eventBus;
        AppState = appState;
        Logger = logger;
        OBSClient = obsClient;
        Encryptor = encryptor;
    }
    
    public Task SetupPlugin() => Task.CompletedTask;

    public async Task StartPlugin()
    {
        OBSClient.OnUnhandledEvent
            .Subscribe(x => Logger.Error($"Unhandled Event in OBS: {x}"))
            .AddTo(_subs);
        
        OBSClient.OnOBSError
            .Subscribe(x => Logger.Error($"Unhandled Error in OBS: {x}"))
            .AddTo(_subs);
        
        OBSClient.OnSceneChanged
            .Subscribe(x => AppState.SetCurrentSceneName(x.NewSceneName))
            .AddTo(_subs);

        Observable.Merge(OBSClient.OnSourceCreated.ToUnit(), OBSClient.OnSourceDestroyed.ToUnit(), OBSClient.OnSourceRenamed.ToUnit())
            .Subscribe(x => RefreshOBSData())
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
        { Logger.Information(result.message); }
    }

    public async Task RefreshOBSData()
    {
        if (!OBSClient.IsConnected)
        { await ConnectToOBS(); }

        if (!OBSClient.IsConnected) { return; }
        var scene = await OBSClient.GetCurrentScene();
        AppState.SetCurrentSceneName(scene.Name);
        await OBSClient.PopulateSourceListData(AppState);
    }

    public void Dispose()
    { _subs?.Dispose(); }
}