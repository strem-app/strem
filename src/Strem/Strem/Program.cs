using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;
using Serilog;
using Strem.Core.Events;
using Strem.Core.Extensions;
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
    
    [STAThread]
    public static void Main(string[] args)
    {
        var app = CreateApp(args);
        var logger = app.Services.GetService<ILogger>();
        var eventBus = app.Services.GetService<IEventBus>();

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            logger.Error(error.ExceptionObject.ToString());
            app.MainWindow.OpenAlertWindow("Fatal exception", error.ExceptionObject.ToString());
        };

        eventBus.Receive<ErrorEvent>().Subscribe(x => logger.Error($"[{x.Source}] {x.Message}"));
        logger.Information("Starting Up Strem");
        
        //TODO: Solve this hack with webhost service collection separation
        WebHostHackExtensions.EventBus = eventBus;
        WebHostHackExtensions.Logger = logger;
        
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
        
        logger.Information("Strem Initializing");
        app.Run();
        webHost.StopHost();
        logger.Information("Strem Stopped");
    }
}