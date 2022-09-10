using Strem.Core.Events.Bus;
using Strem.Data;
using Strem.Data.Stores;
using Strem.Portals.Data;
using Strem.Portals.Events;

namespace Strem.Portals.Services.Stores;

public class PortalStore : InMemoryStore<PortalData, Guid>, IPortalStore
{
    public IEventBus EventBus { get; }
    
    public PortalStore(IRepository<PortalData, Guid> repository, IEventBus eventBus) : base(repository)
    { EventBus = eventBus; }

    protected override Guid GetKey(PortalData data) => data.Id;

    protected override void OnAdded(PortalData data)
    { EventBus.PublishAsync(new PortalCreatedEvent(data.Id)); }
    
    protected override void OnRemoved(PortalData data)
    { EventBus.PublishAsync(new PortalRemovedEvent(data.Id)); }
}