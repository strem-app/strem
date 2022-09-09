using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Todos.Data;
using Strem.Todos.Data.Repositories;
using Strem.Todos.Events;

namespace Strem.Todos.Plugin;

public class TodoPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public ITodoRepository TodoRepository { get; }
    public ITodoStore TodoStore { get; }
    public IEventBus EventBus { get; }
    public ILogger<TodoPluginStartup> Logger { get; }

    public TodoPluginStartup(ITodoRepository todoRepository, ITodoStore todoStore, IEventBus eventBus, ILogger<TodoPluginStartup> logger)
    {
        TodoRepository = todoRepository;
        TodoStore = todoStore;
        EventBus = eventBus;
        Logger = logger;
    }

    public async Task StartPlugin()
    {
        EventBus.Receive<TodoCreatedEvent>()
            .Subscribe(e =>
            {
                var todo = TodoStore.Todos.SingleOrDefault(x => x.Id == e.TodoId);
                if(todo == null) { return; }
                TodoRepository.Create(todo.Id, todo);
            })
            .AddTo(_subs);
        
        EventBus.Receive<TodoRemovedEvent>()
            .Subscribe(x => TodoRepository.Delete(x.TodoId))
            .AddTo(_subs);
    }
    
    public void Dispose()
    {
        _subs.Dispose();
    }
}