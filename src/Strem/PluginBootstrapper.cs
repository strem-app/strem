using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Extensions;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Registries.Integrations;
using Strem.Core.Flows.Registries.Tasks;
using Strem.Core.Flows.Registries.Triggers;
using Strem.Core.Plugins;

namespace Strem;

public class PluginBootstrapper
{
    public IServiceProvider Services { get; }

    public PluginBootstrapper(IServiceProvider services)
    {
        Services = services;
    }

    public async Task StartAllPlugins(IServiceProvider services)
    {
        var startupServices = services.GetServices<IPluginStartup>();
        foreach (var startupService in startupServices)
        { await startupService.StartPlugin(); }
    }

    public async Task StartExecutionEngine(IServiceProvider services)
    {
        var executionEngine = services.GetService<IFlowExecutionEngine>();

        var taskRegistry = services.GetService<ITaskRegistry>();
        var taskDescriptors = services.GetServices<TaskDescriptor>();
        taskRegistry?.AddMany(taskDescriptors);
        
        var triggerRegistry = services.GetService<ITriggerRegistry>();
        var triggerDescriptors = services.GetServices<TriggerDescriptor>();
        triggerRegistry?.AddMany(triggerDescriptors);

        var integrationRegistry = services.GetService<IIntegrationRegistry>();
        var integrationDescriptors = services.GetServices<IIntegrationDescriptor>();
        integrationRegistry?.AddMany(integrationDescriptors);
        
        executionEngine?.StartEngine();
    }
    
    public static void LoadExternalPlugins(List<string> startupErrors)
    {
        var pluginsDirectory = "Plugins";
        if (!Directory.Exists(pluginsDirectory)) { return; }
        
        var pluginFiles = Directory.GetFileSystemEntries(pluginsDirectory, "*.dll");
        if(pluginFiles.Length == 0) { return; }

        foreach (var pluginFile in pluginFiles)
        {
            try { Assembly.LoadFile(pluginFile); }
            catch(Exception ex) { startupErrors.Add($"Failed to load Plugin {pluginFile}: {ex.Message}"); }
        }
    }
}