using Strem.Core.Events.Bus;
using Strem.Data;
using Strem.Data.Stores;
using Strem.Flows.Data;
using Strem.Flows.Events;

namespace Strem.Flows.Services.Stores;

public class FlowStore : InMemoryStore<Flow, Guid>, IFlowStore
{
    public IEventBus EventBus { get; }
    
    public FlowStore(IRepository<Flow, Guid> repository, IEventBus eventBus) : base(repository)
    {
        EventBus = eventBus;
    }

    protected override Guid GetKey(Flow data) => data.Id;

    protected override void OnAdded(Flow data)
    { EventBus.PublishAsync(new FlowAddedEvent(data.Id)); }
    
    protected override void OnRemoved(Flow data)
    { EventBus.PublishAsync(new FlowRemovedEvent(data.Id)); }
}