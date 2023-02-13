using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Core.Variables;

namespace Strem.StreamElements.Plugin;

public class StreamElementsPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public IApplicationConfig ApplicationConfig { get; }
    public ILogger<StreamElementsPluginStartup> Logger { get; }

    public string[] RequiredConfigurationKeys { get; } = Array.Empty<string>();

    public StreamElementsPluginStartup(IEventBus eventBus, IAppState appState, ILogger<StreamElementsPluginStartup> logger, IApplicationConfig applicationConfig)
    {
        EventBus = eventBus;
        AppState = appState;
        Logger = logger;
        ApplicationConfig = applicationConfig;
    }
    
    public Task SetupPlugin() => Task.CompletedTask;
    
    public async Task StartPlugin()
    {
        
    }

    public void Dispose()
    { _subs?.Dispose(); }
}