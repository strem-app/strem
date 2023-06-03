using System.Reflection;
using GlobExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.Types;
using Strem.Core.Variables;
using Strem.Flows.Default.Plugins;
using Strem.Flows.Plugins;
using Strem.Infrastructure.Plugin;
using Strem.Infrastructure.Services;
using Strem.Platforms.Windows.Plugin;
using Strem.Portals.Plugin;
using Strem.Todos.Plugin;
using Strem.Twitch.Plugin;
using Strem.OBS.Plugin;
using Strem.StreamElements.Plugin;
using Strem.Twitter.Plugin;

namespace Strem.Application;

public class PluginHandler : IPluginRegistry
{
    public List<PluginInfo> PluginInfo { get; } = new();
    public IReadOnlyCollection<PluginInfo> GetPluginInfo() => PluginInfo;

    public PluginHandler()
    {
        AppDomain.CurrentDomain.AssemblyResolve += ProxyAssemblyResolves;
    }
    
    Assembly ProxyAssemblyResolves(object sender, ResolveEventArgs args)
    {
        var name = new AssemblyName(args.Name);
        if (args.RequestingAssembly.Location.Contains(StremPathHelper.PluginPath))
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var matchingAssembly = assemblies.SingleOrDefault(x => x.GetName().Name == name.Name);
            return matchingAssembly;
        }
        return null;
    }

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
        _ = typeof(TwitterModule).Assembly;
        _ = typeof(StreamElementsModule).Assembly;
    }

    public IEnumerable<string> PreLoadDynamicPlugins()
    {
        if (!Directory.Exists(StremPathHelper.PluginPath))
        {
            Directory.CreateDirectory(StremPathHelper.PluginPath);
            return Array.Empty<string>();
        }

        var pluginFiles = Glob.Files($"{StremPathHelper.PluginPath}", "**/*.dll").ToArray();
        if(pluginFiles.Length == 0) { return Array.Empty<string>(); }

        var outputLogs = new List<string>();
        foreach (var pluginFile in pluginFiles)
        {
            var fullPath = $"{StremPathHelper.PluginPath}/{pluginFile}";
            outputLogs.Add($"Found Plugin {pluginFile} - Attempting To Load [{fullPath}]");
            try
            {
                Assembly.LoadFrom(fullPath);
            }
            catch (Exception ex)
            {
                outputLogs.Add($"Failed to load Plugin {pluginFile}: {ex.Message}");
                continue;
            }
            outputLogs.Add($"Loaded Dynamic Plugin {pluginFile}");
        }

        return outputLogs;
    }

    public void LoadPluginDependencies(IServiceCollection services, IApplicationConfig appConfig, List<string> preLaunchLogs)
    {
        var dependencyModuleTypes = AppDomain.CurrentDomain.GetAllTypesImplementing<IDependencyModule>();
        foreach (var moduleType in dependencyModuleTypes)
        {
            var module = (IDependencyModule)Activator.CreateInstance(moduleType)!;
            var pluginName = moduleType.Name;
            var hasAllConfigKeys = module.RequiredConfigurationKeys.Length <= 0 || module.RequiredConfigurationKeys.All(appConfig.ContainsKey);
            if (!hasAllConfigKeys)
            {
                var message = $"Cannot find all required config keys for {pluginName}, cannot register dependencies for it";
                preLaunchLogs.Add(message);
                continue;
            }   
            services.AddModule(module);
            preLaunchLogs.Add($"Processed Dependencies For {pluginName} - Met All {module.RequiredConfigurationKeys.Length} Config Requirements");
        }
    }
    
    public async Task StartPlugins(IServiceProvider services, ILogger logger, IApplicationConfig appConfig)
    {
        var startupServices = services.GetServices<IPluginStartup>().ToArray();
        var pluginLoadState = new Dictionary<Type, PluginLoadState>();
        
        // Setup plugins
        foreach (var startupService in startupServices)
        {
            var pluginType = startupService.GetType();
            pluginLoadState.Add(pluginType, PluginLoadState.Unloaded);
            
            var pluginName = startupService.GetType().Name;
            logger.Information($"Setting Up {pluginName}");
            try
            {
                await startupService.SetupPlugin();
                logger.Information($"Finished Setup {pluginName}");
            }
            catch (Exception e)
            {
                logger.Error($"Failed To Setup {pluginName}, with error: {e.Message}");
                pluginLoadState[pluginType] = PluginLoadState.FailedSetup;
            }
        }
        
        // Start plugins
        foreach (var startupService in startupServices)
        {
            var pluginType = startupService.GetType();
            var pluginName = pluginType.Name;

            if (pluginLoadState[pluginType] == PluginLoadState.FailedSetup) { continue; }
            var hasAllConfigKeys = startupService.RequiredConfigurationKeys.Length <= 0 || startupService.RequiredConfigurationKeys.All(appConfig.ContainsKey);
            if (!hasAllConfigKeys)
            {
                var message = $"Cannot find all required config keys for {pluginName}, cannot start it";
                logger.Error(message);
                pluginLoadState[pluginType] = PluginLoadState.FailedStartup;
                continue;
            }
            
            logger.Information($"Starting Plugin {pluginName}");
            try
            {
                await startupService.StartPlugin();
                logger.Information(
                    $"Started Plugin {pluginName}  - Met All {startupService.RequiredConfigurationKeys.Length} Config Requirements");
                pluginLoadState[pluginType] = PluginLoadState.Loaded;
            }
            catch (Exception e)
            {
                logger.Error($"Failed To Start {pluginName}, with error: {e.Message}");
                pluginLoadState[pluginType] = PluginLoadState.FailedStartup;
            }
        }
        
        // Process Plugin Info
        foreach (var loadState in pluginLoadState)
        {
            var pluginAssembly = loadState.Key.Assembly;
            var assemblyInfo = pluginAssembly.GetName();
            var pluginInfo = new PluginInfo(assemblyInfo.Name, assemblyInfo.Version.ToString(), loadState.Value);
            PluginInfo.Add(pluginInfo);
        }
    }
    
    public void StopPlugins(IServiceProvider services, ILogger logger)
    {
        var startupServices = services.GetServices<IPluginStartup>().ToArray();
        
        // Stop plugins
        foreach (var startupService in startupServices)
        {
            var pluginName = startupService.GetType().Name;
            logger.Information($"Stopping {pluginName}");
            try
            {
                startupService.Dispose();
                logger.Information($"Finished Stopping {pluginName}");
            }
            catch (Exception e)
            {
                logger.Error($"Failed To Stop {pluginName}, with error: {e.Message}");
            }
        }
    }
}