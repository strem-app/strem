using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Portals.Data;
using Strem.Portals.Events;
using Strem.Portals.Services.Repositories;
using Strem.Portals.Services.Stores;

namespace Strem.Portals.Plugin;

public class PortalsPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public IPortalRepository PortalRepository { get; }
    public IPortalStore PortalStore { get; }
    public IEventBus EventBus { get; }
    public ILogger<PortalsPluginStartup> Logger { get; }

    public PortalsPluginStartup(IPortalRepository portalRepository, IPortalStore portalStore, ILogger<PortalsPluginStartup> logger, IEventBus eventBus)
    {
        PortalRepository = portalRepository;
        PortalStore = portalStore;
        Logger = logger;
        EventBus = eventBus;
    }

    public async Task StartPlugin()
    {

    }

    public void Dispose()
    { _subs.Dispose(); }
}