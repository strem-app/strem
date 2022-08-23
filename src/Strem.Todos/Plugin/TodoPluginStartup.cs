using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Plugins;
using Strem.Infrastructure.Services.Persistence;
using Strem.Todos.Data;
using Strem.Todos.Events;
using Strem.Todos.Services.Persistence;

namespace Strem.Todos.Plugin;

public class TodoPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public ISaveTodoStorePipeline SavePipeline { get; }
    public ITodoStore TodoStore { get; }
    public IEventBus EventBus { get; }

    public TodoPluginStartup(ISaveTodoStorePipeline savePipeline, ITodoStore todoStore, IEventBus eventBus)
    {
        SavePipeline = savePipeline;
        TodoStore = todoStore;
        EventBus = eventBus;
    }

    public async Task StartPlugin()
    {
        Observable.Interval(TimeSpan.FromMinutes(5))
            .Subscribe(x =>
            {
                SavePipeline.Execute(TodoStore);
            })
            .AddTo(_subs);

        EventBus.Receive<TodoCreatedEvent>()
            .Throttle(TimeSpan.FromSeconds(5))
            .Subscribe(x => SavePipeline.Execute(TodoStore))
            .AddTo(_subs);
        
        EventBus.Receive<TodoRemovedEvent>()
            .Throttle(TimeSpan.FromSeconds(5))
            .Subscribe(x => SavePipeline.Execute(TodoStore))
            .AddTo(_subs);
    }

    public void Dispose()
    {
        _subs.Dispose();
    }
}