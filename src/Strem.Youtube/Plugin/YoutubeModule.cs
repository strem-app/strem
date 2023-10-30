using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Plugins;
using Strem.Core.Services.Registries.Integrations;
using Strem.Flows.Extensions;
using Strem.Infrastructure.Services.Api;
using Strem.Youtube.Services.Client;
using Strem.Youtube.Services.OAuth;

namespace Strem.Youtube.Plugin;

public class YoutubeModule : IRequiresApiHostingModule
{
    public void Setup(IServiceCollection services)
    {
        // Plugin
        services.AddSingleton<IPluginStartup, YoutubePluginStartup>();
        
        // OAuth
        services.AddSingleton<IYoutubeOAuthClient, YoutubeOAuthClient>();
        
        // General
        services.AddSingleton<AccessTokenUserCredential>();
        services.AddSingleton<YouTubeService>(CreateYoutubeClient);
        services.AddSingleton<IObservableYoutubeClient, ObservableYoutubeClient>();
        
        // Components
        var thisAssembly = GetType().Assembly;
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
        
        // Integration Components
        services.AddSingleton<IIntegrationDescriptor, YoutubeIntegrationDescriptor>();
    }
    
    public YouTubeService CreateYoutubeClient(IServiceProvider services)
    {
        var credentialHandler = services.GetService<AccessTokenUserCredential>();
        return new YouTubeService(new BaseClientService.Initializer(){ HttpClientInitializer = credentialHandler });
    }
}