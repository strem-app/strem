﻿using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Plugins;
using Strem.Todos.Services.Repositories;
using Strem.Todos.Services.Stores;

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
    
    public Task SetupPlugin() => Task.CompletedTask;

    public async Task StartPlugin()
    {

    }
    
    public void Dispose()
    {
        _subs.Dispose();
    }
}