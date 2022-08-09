using System.Reactive.Disposables;
using Strem.Core.Events.Bus;
using Strem.Core.Plugins;
using Strem.Core.State;

namespace Strem.OBS.Plugin;

public class OBSPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
 

    public async Task StartPlugin()
    {
     
    }


    public void Dispose()
    { _subs?.Dispose(); }
}