using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino.Blazor;
using PhotinoNET;
using Strem.Core.DI;
using Strem.Core.Events;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Flows.Default.Modules;
using Strem.Infrastructure.Extensions;
using Strem.Infrastructure.Plugin;
using Strem.Infrastructure.Services.Api;
using Strem.OBS.Plugin;
using Strem.Portals.Plugin;
using Strem.Todos.Plugin;
using Strem.Twitch.Plugin;
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
        _ = typeof(PortalsModule).Assembly;
        _ = typeof(TodoModule).Assembly;
        _ = typeof(DefaultFlowsModule).Assembly;
        _ = typeof(TwitchModule).Assembly;
        _ = typeof(OBSModule).Assembly;

        PluginBootstrapper.LoadExternalPlugins(StartupErrors);
    }
    
    public static PhotinoBlazorApp CreateApp(string[] args)
    {
        LoadAllPluginModules();
        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);
        var dependencyModules = AppDomain.CurrentDomain.GetAllTypesImplementing<IDependencyModule>();
        appBuilder.Services.AddModules(dependencyModules);
        appBuilder.RootComponents.Add<App>("#app");
        return appBuilder.Build();
    }

    public static void RunApp(PhotinoBlazorApp app)
    {
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
        Task.Run(() => AppBootstrapper.StartAppSetup(app, appLauncher, StartupErrors.ToArray()));
        app.Run();
    }

    [STAThread]
    public static void Main(string[] args)
    {
        var app = CreateApp(args);
        RunApp(app);
    }
}