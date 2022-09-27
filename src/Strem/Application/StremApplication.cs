using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Strem.Config;
using Strem.Core.Events;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Variables;
using Strem.Infrastructure.Extensions;
using Strem.Infrastructure.Services.Api;
using Strem.Twitch.Plugin;

namespace Strem.Application;

public class StremApplication
{
    public List<string> PreStartupLogs { get; } = new();
    public PluginHandler PluginHandler { get; } = new();
    public BackupHandler BackupHandler { get; } = new();
    
    public IEventBus EventBus { get; private set; }
    public IApplicationConfig AppConfig { get; private set; }
    public ILogger<StremApplication> Logger { get; private set; }
    public IInternalWebHost WebHost { get; private set; }
    public ILiteDatabase Database { get; private set; }
    
    public bool HasStarted { get; private set; }

    public StremApplication()
    {
        BackupHandler.CheckAndBackupIfNeeded(PreStartupLogs);
        
        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            var message = error?.ExceptionObject?.ToString() ?? "Unknown Error";
            if(HasStarted) { Logger?.Error(message); }
            else { PreStartupLogs.Add(message);}
        };
    }

    public void RegisterConfiguration()
    {
        AppConfig.Add(TwitchPluginSettings.TwitchClientIdKey, ConfigData.TwitchClientId);
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
        Logger = services.GetService<ILogger<StremApplication>>()!;
        AppConfig = services.GetService<IApplicationConfig>()!;
        EventBus = services.GetService<IEventBus>()!;
        WebHost = services.GetService<IInternalWebHost>()!;
        Database = services.GetService<ILiteDatabase>()!;

        HasStarted = true;
        PreStartupLogs.ForEach(Logger.Information);
        
        RegisterConfiguration();
        
        Logger.Information("Starting Registered Plugins");
        await PluginHandler.StartPlugins(services, Logger, AppConfig);
        Logger.Information("Started Registered Plugins");
        
        //TODO: Solve this hack with webhost service collection separation
        WebHostHackExtensions.EventBus = EventBus;
        WebHostHackExtensions.ServiceLocator = services;
        
        Logger.Information("Starting Internal Host");
        WebHost.StartHost();
        Logger.Information($"Started Internal Host: http://localhost:{InternalWebHostConfiguration.ApiHostPort}");
        
        Logger.Information("Strem Initialized");
        EventBus.PublishAsync(new ApplicationStartedEvent());
    }
    
    public bool ShutdownApp()
    {
        EventBus?.Publish(new ApplicationClosingEvent());
        Database?.Dispose();
        if(WebHost?.IsRunning ?? false) { WebHost.StopHost(); }
        Logger?.Information("Strem Stopped");
        return false;
    }
}