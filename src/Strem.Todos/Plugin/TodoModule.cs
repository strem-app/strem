using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.Services.Registries.Menus;
using Strem.Data;
using Strem.Flows.Extensions;
using Strem.Todos.Data;
using Strem.Todos.Services.Repositories;
using Strem.Todos.Services.Stores;

namespace Strem.Todos.Plugin;

public class TodoModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // Startup
        services.AddSingleton<IPluginStartup, TodoPluginStartup>();
        
        // Menus
        services.AddSingleton(new MenuDescriptor
        {
            Priority = 3,
            Title = "Todos",
            Code = "todos-menu",
            IconClass = "fas fa-clipboard-list",
            PageUrl = "todos"
        });
        
        // Persistence
        services.AddSingleton<ITodoRepository, IRepository<TodoData, Guid>, TodoRepository>();
        services.AddSingleton<ITodoStore, TodoStore>();
        
        // Components/Flows
        var thisAssembly = GetType().Assembly;
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
    }
}