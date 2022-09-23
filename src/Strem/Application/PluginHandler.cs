using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.Variables;
using Strem.Flows.Default.Modules;
using Strem.Flows.Plugins;
using Strem.Infrastructure.Plugin;
using Strem.OBS.v4.Plugin;
using Strem.Platforms.Windows.Plugin;
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
        _ = typeof(WindowsPlatformModule).Assembly;
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
    
    public async Task StartPlugins(IServiceProvider services, ILogger logger, IApplicationConfig appConfig)
    {
        var startupServices = services.GetServices<IPluginStartup>().ToArray();

        // Setup plugins
        foreach (var startupService in startupServices)
        {
            var pluginName = startupService.GetType().Name;
            logger.Information($"Setting Up {pluginName}");
            await startupService.SetupPlugin();
            logger.Information($"Finished Setup {pluginName}");
        }
        
        // Start plugins
        foreach (var startupService in startupServices)
        {
            var pluginName = startupService.GetType().Name;
            var hasAllConfigKeys = startupService.RequiredConfigurationKeys.Length <= 0 || startupService.RequiredConfigurationKeys.All(appConfig.ContainsKey);
            if (!hasAllConfigKeys)
            {
                var message = $"Cannot find all required config keys for {pluginName}, cannot start it";
                logger.Error(message);
                continue;
            }
            
            logger.Information($"Starting Plugin {pluginName}");
            await startupService.StartPlugin();
            logger.Information($"Started Plugin {pluginName}");
        }
    }
}