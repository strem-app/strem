using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Plugins;
using Strem.Infrastructure.Services.Persistence;
using Strem.Todos.Data;
using Strem.Todos.Services.Persistence;

namespace Strem.Todos.Plugin;

public class TodoPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public ISaveTodoStorePipeline SavePipeline { get; set; }
    public ITodoStore TodoStore { get; set; }

    public TodoPluginStartup(ISaveTodoStorePipeline savePipeline, ITodoStore todoStore)
    {
        SavePipeline = savePipeline;
        TodoStore = todoStore;
    }

    public async Task StartPlugin()
    {
        Observable.Interval(TimeSpan.FromMinutes(5))
            .Subscribe(x =>
            {
                SavePipeline.Execute(TodoStore);
            })
            .AddTo(_subs);
    }

    public void Dispose()
    {
        _subs.Dispose();
    }
}