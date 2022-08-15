using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino.Blazor;
using Strem.Core.DI;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Flows.Default.Modules;
using Strem.Infrastructure.Extensions;
using Strem.Infrastructure.Modules;
using Strem.Infrastructure.Services.Api;
using Strem.OBS.Modules;
using Strem.Twitch.Modules;
using Strem.UI;

namespace Strem;

public class Program
{
    public static List<string> StartupErrors { get; } = new();
    
    // TODO: At some point will be replaced by plugins or something
    public static void LoadAllPluginModules()
    {
        Assembly _;
        _ = typeof(InfrastructureModule).Assembly;
        _ = typeof(DefaultFlowsModule).Assembly;
        _ = typeof(TwitchModule).Assembly;
        _ = typeof(OBSModule).Assembly;

        LoadExternalPlugins();
    }

    public static void LoadExternalPlugins()
    {
        var pluginsDirectory = "Plugins";
        if (!Directory.Exists(pluginsDirectory)) { return; }
        
        var pluginFiles = Directory.GetFileSystemEntries(pluginsDirectory, "*.dll");
        if(pluginFiles.Length == 0) { return; }

        foreach (var pluginFile in pluginFiles)
        {
            try { Assembly.LoadFile(pluginFile); }
            catch(Exception ex) { StartupErrors.Add($"Failed to load Plugin {pluginFile}: {ex.Message}"); }
        }
    }
    
    public static Type[] GetAllDependencyModules()
    {
        LoadAllPluginModules();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        return assemblies
            .SelectMany(x => x.GetTypes().WhereClassesImplement(typeof(IDependencyModule)))
            .ToArray();
    }
    
    public static PhotinoBlazorApp CreateApp(string[] args)
    {
        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);
        var dependencyModules = GetAllDependencyModules();
        appBuilder.Services.AddModules(dependencyModules);
        appBuilder.RootComponents.Add<App>("#app");
        return appBuilder.Build();
    }

    [STAThread]
    public static void Main(string[] args)
    {
        var app = CreateApp(args);
        var pluginBootstrapper = new PluginBootstrapper(app.Services);
        var logger = app.Services.GetService<ILogger<Program>>();
        var eventBus = app.Services.GetService<IEventBus>();
        
        logger.Information("Starting Up Strem");
        foreach (var error in StartupErrors)
        { logger.Error(error); }
        
        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            logger.Error(error.ExceptionObject.ToString());
            app.MainWindow.OpenAlertWindow("Fatal exception", error.ExceptionObject.ToString());
        };

        logger.Information("Starting Plugins");
        Task.Run(async () => await pluginBootstrapper.StartAllPlugins(app.Services)).Wait();
        
        //TODO: Solve this hack with webhost service collection separation
        WebHostHackExtensions.EventBus = eventBus;
        WebHostHackExtensions.ServiceLocator = app.Services;
        
        var webHost = app.Services.GetService<IApiWebHost>();
        logger.Information("Starting Internal Host");
        webHost.StartHost();
        logger.Information($"Started Internal Host: http://localhost:{ApiHostConfiguration.ApiHostPort}");

        var appLauncher = app.MainWindow
            .SetTitle("Strem")
            .SetSize(1920, 1080)
            .SetUseOsDefaultSize(false)
            .SetIconFile("strem.ico");
#if DEBUG
        appLauncher = appLauncher.SetDevToolsEnabled(true);
#else
        appLauncher = appLauncher.SetContextMenuEnabled(false);
#endif
        appLauncher.Load("./wwwroot/index.html");
        
        logger.Information("Starting Flow Execution Engine");
        Task.Run(async () => await pluginBootstrapper.StartExecutionEngine(app.Services)).Wait();
        logger.Information("Started Flow Execution Engine");
        
        logger.Information("Strem Initialized");
        app.Run();
        webHost.StopHost();
        logger.Information("Strem Stopped");
    }
}