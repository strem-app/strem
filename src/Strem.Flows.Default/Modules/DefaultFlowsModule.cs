using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.Services.Registries.Menus;
using Strem.Flows.Extensions;

namespace Strem.Flows.Default.Modules;

public class DefaultFlowsModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // Menus
        services.AddSingleton(new MenuDescriptor
        {
            Priority = 1,
            Title = "Flows",
            Code = "flows-menu",
            IconClass = "fas fa-code-commit",
            PageUrl = "flows"
        });
        
        // Components
        var thisAssembly = GetType().Assembly;
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
    }
}