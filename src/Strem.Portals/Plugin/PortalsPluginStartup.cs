using System.Reactive.Disposables;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Portals.Data;
using Strem.Portals.Data.Repositories;
using Strem.Portals.Events;

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
        EventBus.Receive<PortalChangedEvent>()
            .Subscribe(x => HandlePortalChange(x.PortalId))
            .AddTo(_subs);
    }

    public void HandlePortalChange(Guid portalId)
    {
        var portal = PortalStore.Portals.SingleOrDefault(x => x.Id == portalId);
        if (portal == null)
        { PortalRepository.Delete(portalId); }
        else
        { PortalRepository.Upsert(portalId, portal); }
    }

    public void Dispose()
    { _subs.Dispose(); }
}