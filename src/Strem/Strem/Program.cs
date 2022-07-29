using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino.Blazor;
using Strem.Core.Events;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Infrastructure.Extensions;
using Strem.Infrastructure.Modules;
using Strem.Infrastructure.Services.Api;
using Strem.Twitch.Controllers;
using Strem.Twitch.Modules;
using Strem.Web;

namespace Strem;

public class Program
{
    public static PhotinoBlazorApp CreateApp(string[] args)
    {
        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);
        appBuilder.Services.AddModules(new InfrastructureModule(), new TwitchModule());
        appBuilder.RootComponents.Add<App>("#app");
        return appBuilder.Build();
    }

    public static async Task StartAllPlugins(IServiceProvider services)
    {
        var startupServices = services.GetServices<IPluginStartup>();
        foreach (var startupService in startupServices)
        { await startupService.StartPlugin(); }
    }
    
    [STAThread]
    public static void Main(string[] args)
    {
        var app = CreateApp(args);
        var logger = app.Services.GetService<ILogger<Program>>();
        var eventBus = app.Services.GetService<IEventBus>();
        logger.Information("Starting Up Strem");
        
        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            logger.Error(error.ExceptionObject.ToString());
            app.MainWindow.OpenAlertWindow("Fatal exception", error.ExceptionObject.ToString());
        };

        logger.Information("Starting Plugins");
        Task.Run(async () => await StartAllPlugins(app.Services)).Wait();
        
        //TODO: Solve this hack with webhost service collection separation
        WebHostHackExtensions.EventBus = eventBus;
        WebHostHackExtensions.ServiceLocator = app.Services;
        
        var apiHostConfiguration = new ApiHostConfiguration(
            new[] { typeof(TwitchController).Assembly },
            new[] { new InfrastructureModule() });
        
        var webHost = app.Services.GetService<IApiWebHost>();
        logger.Information("Starting Internal Host");
        webHost.StartHost(apiHostConfiguration);
        logger.Information($"Started Internal Host: {ApiWebHost.InternalPort}");
        
        app.MainWindow
            .SetTitle("Strem")
            .SetDevToolsEnabled(true)
            .SetSize(1920, 1080)
            .SetUseOsDefaultSize(false)
            .Load("./wwwroot/index.html");
        
        logger.Information("Strem Initialized");
        app.Run();
        webHost.StopHost();
        logger.Information("Strem Stopped");
    }
}