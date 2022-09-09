using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Extensions;
using Strem.Core.Flows.Registries.Menus;
using Strem.Core.Plugins;
using Strem.Infrastructure.Services.Api;
using Strem.Todos.Data;
using Strem.Todos.Data.Repositories;

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
        services.AddSingleton<ITodoRepository, TodoRepository>();
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
        var repository = services.GetService<ITodoRepository>();
        var allTodos = repository.GetAll();
        return new TodoStore(allTodos);
    }
}