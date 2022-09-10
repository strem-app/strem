using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Strem.Core.DI;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Flows.Default.Modules;
using Strem.Flows.Plugins;
using Strem.Infrastructure.Plugin;
using Strem.OBS.v4.Plugin;
using Strem.Portals.Plugin;
using Strem.Todos.Plugin;
using Strem.Twitch.Plugin;

namespace Strem.Application;

public class PluginHandler
{
    public void PreLoadLocalPlugins()
    {
        Assembly _;
        _ = typeof(FlowsModule).Assembly;
        _ = typeof(DefaultFlowsModule).Assembly;
        _ = typeof(InfrastructureModule).Assembly;
        _ = typeof(PortalsModule).Assembly;
        _ = typeof(TodoModule).Assembly;
        _ = typeof(TwitchModule).Assembly;
        _ = typeof(OBSModule).Assembly;
    }

    public IEnumerable<string> PreLoadDynamicPlugins()
    {
        var pluginsDirectory = "Plugins";
        if (!Directory.Exists(pluginsDirectory)) { return Array.Empty<string>(); }
        
        var pluginFiles = Directory.GetFileSystemEntries(pluginsDirectory, "*.dll");
        if(pluginFiles.Length == 0) { return Array.Empty<string>(); }

        var outputLogs = new List<string>();
        foreach (var pluginFile in pluginFiles)
        {
            try { Assembly.LoadFile(pluginFile); }
            catch(Exception ex) { outputLogs.Add($"Failed to load Plugin {pluginFile}: {ex.Message}"); }
        }

        return outputLogs;
    }

    public IEnumerable<string> LoadPluginDependencies(IServiceCollection services)
    {
        var dependencyModules = AppDomain.CurrentDomain.GetAllTypesImplementing<IDependencyModule>();
        services.AddModules(dependencyModules);
        return dependencyModules.Select(x => $"Processed Dependencies For {x.Name}");
    }
    
    public async Task StartPlugins(IServiceProvider services, ILogger logger)
    {
        var startupServices = services.GetServices<IPluginStartup>();
        foreach (var startupService in startupServices)
        {
            var pluginName = startupService.GetType().Name;
            logger.Information($"Starting {pluginName}");
            await startupService.StartPlugin();
            logger.Information($"Finished {pluginName}");
        }
    }
}