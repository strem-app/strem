using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Extensions;
using Strem.Core.Flows.Registries.Menus;
using Strem.Core.Plugins;
using Strem.Infrastructure.Services.Api;
using Strem.Todos.Data;
using Strem.Todos.Services.Persistence;

namespace Strem.Todos.Plugin;

public class TodoModule : IRequiresApiHostingModule
{
    public void Setup(IServiceCollection services)
    {
        // Menus
        services.AddSingleton(new MenuDescriptor
        {
            Title = "Todos",
            Code = "todos-menu",
            IconClass = "fas fa-clipboard-list",
            PageUrl = "todos"
        });
        
        // Persistence
        services.AddSingleton<ILoadTodoStorePipeline, LoadTodoStorePipeline>();
        services.AddSingleton<ISaveTodoStorePipeline, SaveTodoStorePipeline>();
        services.AddSingleton<ITodoStore>(LoadTodoStore);
        
        // Components/Flows
        var thisAssembly = GetType().Assembly;
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
        
        // Startup
        services.AddSingleton<IPluginStartup, TodoPluginStartup>();
    }
    
    public ITodoStore LoadTodoStore(IServiceProvider services)
    {
        var storeLoader = services.GetService<ILoadTodoStorePipeline>();
        return Task.Run(async () =>
        {
            await CreateStoreFileIfNeeded(services);
            return await storeLoader.Execute();
        }).Result;
    }
    
    public async Task CreateStoreFileIfNeeded(IServiceProvider services)
    {
        var storeSaver = services.GetService<ISaveTodoStorePipeline>();
        var portalStoreFilePath = storeSaver.DataFilePath;
        if (!File.Exists(portalStoreFilePath))
        { await storeSaver.Execute(new TodoStore()); }
    }
}