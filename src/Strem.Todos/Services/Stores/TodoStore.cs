using Strem.Core.Events.Bus;
using Strem.Data;
using Strem.Data.Stores;
using Strem.Todos.Data;
using Strem.Todos.Events;

namespace Strem.Todos.Services.Stores;

public class TodoStore : InMemoryStore<TodoData, Guid>, ITodoStore
{
    public IEventBus EventBus { get; }
    
    public TodoStore(IRepository<TodoData, Guid> repository, IEventBus eventBus) : base(repository)
    { EventBus = eventBus; }

    protected override Guid GetKey(TodoData data) => data.Id;

    protected override void OnAdded(TodoData data)
    { EventBus.PublishAsync(new TodoCreatedEvent(data.Id)); }
    
    protected override void OnRemoved(TodoData data)
    { EventBus.PublishAsync(new TodoRemovedEvent(data.Id)); }
}