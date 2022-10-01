using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;
using Strem.UI;

namespace Strem.Application;

public class BlazorBootstrapper
{
    public PhotinoBlazorApp CreateApp(Action<IServiceCollection> beforeCreated)
    {
        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault();
        appBuilder.RootComponents.Add<App>("#app");
        beforeCreated(appBuilder.Services);
        return appBuilder.Build();
    }

    public void SetupWindow(PhotinoBlazorApp app)
    {
        var appLauncher = app.MainWindow
            .SetTitle("Strem")
            
#if DEBUG
            .SetLogVerbosity(3)
#else
            .SetLogVerbosity(0)
#endif
            
            .SetSize(1920, 1080)
            .SetUseOsDefaultSize(false)
            .SetIconFile("strem.ico");
#if DEBUG
        appLauncher = appLauncher.SetDevToolsEnabled(true);
#else
        appLauncher = appLauncher.SetContextMenuEnabled(false);
#endif
        appLauncher.Load("_content/Strem/wwwroot/index.html");
    }

    public void SetupApp(Action<IServiceCollection> beforeCreated, Func<PhotinoBlazorApp, Task> afterCreated)
    {
        var app = CreateApp(beforeCreated);
        SetupWindow(app);
        Task.Run(() => afterCreated(app));
        app.Run();
    }
}