using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StreamElements.WebSocket.Reactive;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.StreamElements.Extensions;
using Strem.StreamElements.Variables;

namespace Strem.StreamElements.Plugin;

public class StreamElementsPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public IApplicationConfig ApplicationConfig { get; }
    public IObservableStreamElementsClient StreamElementsClient { get; }
    public ILogger<StreamElementsPluginStartup> Logger { get; }

    public string[] RequiredConfigurationKeys { get; } = Array.Empty<string>();

    public StreamElementsPluginStartup(IEventBus eventBus, IAppState appState, ILogger<StreamElementsPluginStartup> logger, IApplicationConfig applicationConfig, IObservableStreamElementsClient streamElementsClient)
    {
        EventBus = eventBus;
        AppState = appState;
        Logger = logger;
        ApplicationConfig = applicationConfig;
        StreamElementsClient = streamElementsClient;
    }
    
    public Task SetupPlugin() => Task.CompletedTask;
    
    public async Task StartPlugin()
    {
        StreamElementsClient.OnConnected
            .Subscribe(x => Logger.Information("Connected to StreamElements"))
            .AddTo(_subs);
        
        StreamElementsClient.OnDisconnected
            .Subscribe(x => Logger.Information("Disconnected from StreamElements"))
            .AddTo(_subs);
        
        StreamElementsClient.OnError
            .Subscribe(x => Logger.LogError(x.Exception, "StreamElements error reported"))
            .AddTo(_subs);
        
        if (AppState.HasJwtToken())
        { await AttemptConnect(); }
    }

    public Task AttemptConnect()
    {
        if (StreamElementsClient.WebSocketClient.IsConnected) { return Task.CompletedTask; }

        var token = AppState.AppVariables.Get(StreamElementsVars.JwtToken);

        var task = Task.WhenAny(StreamElementsClient.OnConnected.FirstAsync().ToTask(), Task.Delay(2000));
        StreamElementsClient.WebSocketClient.Connect(token);

        return task;
    }

    public void Dispose()
    { _subs?.Dispose(); }
}