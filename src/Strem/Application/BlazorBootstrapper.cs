using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;
using Strem.Infrastructure.Services;
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
            .SetJavascriptClipboardAccessEnabled(true)
            .SetFileSystemAccessEnabled(true)
            
#if DEBUG
            .SetLogVerbosity(3)
            .SetDevToolsEnabled(true)
#else
            .SetLogVerbosity(0)
            .SetDevToolsEnabled(false)
            .SetContextMenuEnabled(false)
#endif
            
            .SetSize(1920, 1080)
            .SetUseOsDefaultSize(false)
            .SetIconFile(Path.GetFullPath($"{StremPathHelper.AppPath}/strem.ico"));
    }

    public void SetupApp(Action<IServiceCollection> beforeCreated, Func<PhotinoBlazorApp, Task> afterCreated)
    {
        var app = CreateApp(beforeCreated);
        SetupWindow(app);
        Task.Run(() => afterCreated(app));
        app.Run();
    }
}