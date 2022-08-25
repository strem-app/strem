using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Strem.Core.Events;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Infrastructure.Extensions;
using Strem.Infrastructure.Services.Api;

namespace Strem;

public class StremApplication
{
    public List<string> PreStartupLogs { get; } = new();
    public PluginHandler PluginHandler { get; } = new();
    public ExecutionEngineHandler ExecutionEngineHandler { get; } = new();
    
    public IEventBus EventBus { get; private set; }
    public ILogger<StremApplication> Logger { get; private set; }
    public IInternalWebHost WebHost { get; private set; }
    
    public bool HasStarted { get; private set; }

    public StremApplication()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            var message = error?.ExceptionObject?.ToString() ?? "Unknown Error";
            if(HasStarted) { Logger.Error(message); }
            else { PreStartupLogs.Add(message);}
        };
    }

    public void PreLoadPlugins()
    {
        PluginHandler.PreLoadLocalPlugins();
        var logs = PluginHandler.PreLoadDynamicPlugins();
        PreStartupLogs.AddRange(logs);
    }

    public void LoadPlugins(IServiceCollection services)
    {
        var logs = PluginHandler.LoadPluginDependencies(services);
        PreStartupLogs.AddRange(logs);
    }
    
    public async Task StartApplication(IServiceProvider services)
    {
        Logger = services.GetService<ILogger<StremApplication>>();
        EventBus = services.GetService<IEventBus>();
        WebHost = services.GetService<IInternalWebHost>();

        HasStarted = true;
        PreStartupLogs.ForEach(Logger.Information);
        
        Logger.Information("Starting Registered Plugins");
        await PluginHandler.StartPlugins(services, Logger);
        Logger.Information("Started Registered Plugins");
        
        //TODO: Solve this hack with webhost service collection separation
        WebHostHackExtensions.EventBus = EventBus;
        WebHostHackExtensions.ServiceLocator = services;
        
        Logger.Information("Starting Internal Host");
        WebHost.StartHost();
        Logger.Information($"Started Internal Host: http://localhost:{InternalWebHostConfiguration.ApiHostPort}");
        
        Logger.Information("Starting Flow Execution Engine");
        await ExecutionEngineHandler.StartExecutionEngine(services);
        Logger.Information("Started Flow Execution Engine");
        
        Logger.Information("Strem Initialized");
        EventBus.PublishAsync(new ApplicationStartedEvent());
    }
    
    public bool ShutdownApp()
    {
        EventBus?.Publish(new ApplicationClosingEvent());
        if(WebHost?.IsRunning ?? false) { WebHost.StopHost(); }
        Logger?.Information("Strem Stopped");
        return false;
    }
}