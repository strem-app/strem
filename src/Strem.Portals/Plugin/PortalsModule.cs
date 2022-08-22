﻿using Strem.Core.Extensions;
using Strem.Core.Flows.Registries.Menus;
using Strem.Infrastructure.Services.Api;
using Strem.Portals.Data;
using Strem.Portals.Services.Persistence;

namespace Strem.Portals.Plugin;

public class PortalsModule : IRequiresApiHostingModule
{
    public void Setup(IServiceCollection services)
    {
        // Menus
        services.AddSingleton(new MenuDescriptor
        {
            Title = "Portals",
            Code = "portals-menu",
            IconClass = "fas fa-globe",
            PageUrl = "portals"
        });
        
        // Persistence
        services.AddSingleton<ILoadPortalStorePipeline, LoadPortalStorePipeline>();
        services.AddSingleton<ISavePortalStorePipeline, SavePortalStorePipeline>();
        services.AddSingleton<IPortalStore>(LoadPortalStore);
        
        // Components/Flows
        var thisAssembly = GetType().Assembly;
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
    }
    
    public IPortalStore LoadPortalStore(IServiceProvider services)
    {
        var portalStoreLoader = services.GetService<ILoadPortalStorePipeline>();
        return Task.Run(async () =>
        {
            await CreateStoreFileIfNeeded(services);
            return await portalStoreLoader.Execute();
        }).Result;
    }
    
    public async Task CreateStoreFileIfNeeded(IServiceProvider services)
    {
        var portalStoreSaver = services.GetService<ISavePortalStorePipeline>();
        var portalStoreFilePath = portalStoreSaver.DataFilePath;
        if (!File.Exists(portalStoreFilePath))
        { await portalStoreSaver.Execute(new PortalStore()); }
    }
}