using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;
using Strem.Application;

namespace Strem;

public class Program
{
    public static StremApplication Application { get; } = new();
    public static BlazorBootstrapper BlazorBootstrapper { get; } = new();

    [STAThread]
    public static void Main(string[] args)
    {
        Application.PreStartupLogs.Add("Pre loading Plugin Assemblies");
        Application.PreLoadPlugins();
        Application.PreStartupLogs.Add("Initializing Blazor Host App");
        BlazorBootstrapper.SetupApp(OnBlazorAppStarting, OnBlazorStarted);
    }

    private static void OnBlazorAppStarting(IServiceCollection services)
    {
        Application.PreStartupLogs.Add("Loading All Plugin Modules");
        Application.LoadPlugins(services);
    }

    public static async Task OnBlazorStarted(PhotinoBlazorApp blazorApp)
    {
        Application.PreStartupLogs.Add("Blazor Host App Started");
        blazorApp.MainWindow.WindowClosing += (sender, args) => Application.ShutdownApp();
        await Application.StartApplication(blazorApp.Services);
    }
}