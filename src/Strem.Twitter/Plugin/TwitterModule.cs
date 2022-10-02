using Strem.Core.Plugins;
using Strem.Core.Services.Registries.Integrations;
using Strem.Flows.Extensions;
using Strem.Infrastructure.Services.Api;
using Strem.Twitter.Services.ApiClient;
using Strem.Twitter.Services.OAuth;

namespace Strem.Twitter.Plugin;

public class TwitterModule : IRequiresApiHostingModule
{
    public void Setup(IServiceCollection services)
    {
        // Plugin
        services.AddSingleton<IPluginStartup, TwitterPluginStartup>();
        
        // OAuth
        services.AddSingleton<ITwitterOAuthClient, TwitterOAuthClient>();
        
        // Api
        services.AddSingleton<ITwitterApiClient, TwitterApiClient>();
        
        // Components
        var thisAssembly = GetType().Assembly;
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
        
        // Integration Components
        services.AddSingleton<IIntegrationDescriptor, TwitterIntegrationDescriptor>();
    }
}