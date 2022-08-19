using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino.Blazor;
using PhotinoNET;
using Strem.Core.DI;
using Strem.Core.Events;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Infrastructure.Extensions;
using Strem.Infrastructure.Services.Api;

namespace Strem;

public class AppBootstrapper
{
    public static Type[] GetAllDependencyModules()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        return assemblies
            .SelectMany(x => x.GetTypes().WhereClassesImplement(typeof(IDependencyModule)))
            .ToArray();
    }
    
    public static void StartAppSetup(PhotinoBlazorApp app, PhotinoWindow window, string[] startupErrors)
    {
        var pluginBootstrapper = new PluginBootstrapper(app.Services);
        var logger = app.Services.GetService<ILogger<Program>>();
        var eventBus = app.Services.GetService<IEventBus>();
        var webHost = app.Services.GetService<IApiWebHost>();
        
        window.WindowClosing += (sender, args) =>
        {
            eventBus.Publish(new ApplicationClosingEvent());
            if(webHost.IsRunning) { webHost.StopHost(); }
            logger.Information("Strem Stopped");
            return false;
        };
        
        logger.Information("Starting Up Strem");
        foreach (var error in startupErrors)
        { logger.Error(error); }
        
        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            logger.Error(error?.ExceptionObject?.ToString() ?? "Unknown Error");
            app.MainWindow.OpenAlertWindow("Fatal exception", error?.ExceptionObject?.ToString() ?? "Unknown Error");
        };

        logger.Information("Starting Plugins");
        Task.Run(async () => await pluginBootstrapper.StartAllPlugins(app.Services)).Wait();
        
        //TODO: Solve this hack with webhost service collection separation
        WebHostHackExtensions.EventBus = eventBus;
        WebHostHackExtensions.ServiceLocator = app.Services;
        
        logger.Information("Starting Internal Host");
        webHost.StartHost();
        logger.Information($"Started Internal Host: http://localhost:{ApiHostConfiguration.ApiHostPort}");
        
        logger.Information("Starting Flow Execution Engine");
        Task.Run(async () => await pluginBootstrapper.StartExecutionEngine(app.Services)).Wait();
        logger.Information("Started Flow Execution Engine");
        
        logger.Information("Strem Initialized");
        eventBus.PublishAsync(new ApplicationStartedEvent());
    }
}