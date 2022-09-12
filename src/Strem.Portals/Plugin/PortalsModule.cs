using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.Services.Registries.Menus;
using Strem.Data;
using Strem.Flows.Extensions;
using Strem.Infrastructure.Services.Api;
using Strem.Portals.Data;
using Strem.Portals.Data.Overrides;
using Strem.Portals.Services.Data;
using Strem.Portals.Services.Repositories;
using Strem.Portals.Services.Stores;

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
        services.AddSingleton<IPortalRepository, IRepository<PortalData, Guid>, PortalRepository>();
        services.AddSingleton<IPortalStore, PortalStore>();
        services.AddSingleton<IPortalImporter, PortalImporter>();
        services.AddSingleton<IPortalExporter, PortalExporter>();
        services.AddSingleton<ButtonRuntimeStyles>(PopulateRuntimeStyles);
        
        // Components/Flows
        var thisAssembly = GetType().Assembly;
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
        
        // Startup
        services.AddSingleton<IPluginStartup, PortalsPluginStartup>();
    }
    
    public ButtonRuntimeStyles PopulateRuntimeStyles(IServiceProvider services)
    {
        var portalStore = services.GetService<IPortalStore>();
        var buttonRuntimeStyles = new ButtonRuntimeStyles();
        foreach (var portal in portalStore.Data)
        {
            var buttonStyles = portal.Buttons
                .ToDictionary(x => x.Id, x => new ButtonStyles(x.DefaultStyles));
            buttonRuntimeStyles.RuntimeStyles.Add(portal.Id, buttonStyles);
        }
        return buttonRuntimeStyles;
    }
}