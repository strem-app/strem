﻿using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Strem.Config;
using Strem.Core.Events;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Variables;
using Strem.Infrastructure.Extensions;
using Strem.Infrastructure.Plugin;
using Strem.Infrastructure.Services.Api;
using Strem.Twitch.Plugin;
using Strem.Twitter.Plugin;

namespace Strem.Application;

public class StremApplication
{
    public List<string> PreStartupLogs { get; } = new();
    public PluginHandler PluginHandler { get; } = new();
    public BackupHandler BackupHandler { get; } = new();
    
    public IEventBus EventBus { get; private set; }
    public IApplicationConfig AppConfig { get; } = new ApplicationConfig();
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
        AppConfig.Add(TwitterPluginSettings.TwitterClientIdKey, ConfigData.TwitterClientId);
        AppConfig.Add(InfrastructurePluginSettings.EncryptionKeyKey, ConfigData.EncryptionKey);
        AppConfig.Add(InfrastructurePluginSettings.EncryptionIVKey, ConfigData.EncryptionIV);
    }

    public void PreLoadPlugins()
    {
        PluginHandler.PreLoadLocalPlugins();
        var logs = PluginHandler.PreLoadDynamicPlugins();
        PreStartupLogs.AddRange(logs);
    }

    public void LoadPlugins(IServiceCollection services)
    {
        // Config
        PreStartupLogs.Add("Setting up Application Config");
        services.AddSingleton<IApplicationConfig>(AppConfig);
        RegisterConfiguration();
        
        PluginHandler.LoadPluginDependencies(services, AppConfig, PreStartupLogs);
    }
    
    public async Task StartApplication(IServiceProvider services)
    {
        Logger = services.GetService<ILogger<StremApplication>>()!;
        EventBus = services.GetService<IEventBus>()!;
        WebHost = services.GetService<IInternalWebHost>()!;
        Database = services.GetService<ILiteDatabase>()!;

        HasStarted = true;
        PreStartupLogs.ForEach(Logger.Information);
        
        Logger.Information("Starting Registered Plugins");
        await PluginHandler.StartPlugins(services, Logger, AppConfig);
        Logger.Information("Started Registered Plugins");
        
        // This is a publish on purpose to hold execution
        EventBus.Publish(new ApplicationPluginsLoadedEvent());
        
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