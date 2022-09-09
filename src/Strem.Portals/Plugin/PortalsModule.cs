using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.Services.Registries.Menus;
using Strem.Flows.Extensions;
using Strem.Infrastructure.Services.Api;
using Strem.Portals.Data;
using Strem.Portals.Data.Overrides;
using Strem.Portals.Data.Repositories;

namespace Strem.Portals.Plugin;

public class PortalsModule : IRequiresApiHostingModule
{
    public void Setup(IServiceCollection services)
    {
        // Menus
        services.AddSingleton(new MenuDescriptor
        {
            Priority = 2,
            Title = "Portals",
            Code = "portals-menu",
            IconClass = "fas fa-globe",
            PageUrl = "portals"
        });
        
        // Data
        services.AddSingleton<IPortalRepository, PortalRepository>();
        services.AddSingleton<IPortalStore>(LoadPortalStore);
        services.AddSingleton<ButtonRuntimeStyles>(PopulateRuntimeStyles);
        
        // Components/Flows
        var thisAssembly = GetType().Assembly;
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
        
        // Startup
        services.AddSingleton<IPluginStartup, PortalsPluginStartup>();
    }
    
    public IPortalStore LoadPortalStore(IServiceProvider services)
    {
        var portalRepository = services.GetService<IPortalRepository>();
        var allPortals = portalRepository.GetAll();
        return new PortalStore(allPortals);
    }
    
    public ButtonRuntimeStyles PopulateRuntimeStyles(IServiceProvider services)
    {
        var portalStore = services.GetService<IPortalStore>();
        var buttonRuntimeStyles = new ButtonRuntimeStyles();
        foreach (var portal in portalStore.Portals)
        {
            var buttonStyles = portal.Buttons
                .ToDictionary(x => x.Id, x => new ButtonStyles(x.DefaultStyles));
            buttonRuntimeStyles.RuntimeStyles.Add(portal.Id, buttonStyles);
        }
        return buttonRuntimeStyles;
    }
}