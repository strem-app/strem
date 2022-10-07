using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Twitter.Events.OAuth;
using Strem.Twitter.Extensions;
using Strem.Twitter.Services.ApiClient;
using Strem.Twitter.Services.OAuth;
using Strem.Twitter.Variables;

namespace Strem.Twitter.Plugin;

public class TwitterPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public ITwitterOAuthClient TwitterOAuthClient { get; }
    public ITwitterApiClient TwitterApiClient { get; }
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public ILogger<TwitterPluginStartup> Logger { get; }

    public string[] RequiredConfigurationKeys { get; } = new[] { TwitterPluginSettings.TwitterClientIdKey };

    public TwitterPluginStartup(ITwitterOAuthClient twitterOAuthClient, IEventBus eventBus, IAppState appState, ILogger<TwitterPluginStartup> logger, ITwitterApiClient twitterApiClient)
    {
        TwitterOAuthClient = twitterOAuthClient;
        EventBus = eventBus;
        AppState = appState;
        Logger = logger;
        TwitterApiClient = twitterApiClient;
    }

    public Task SetupPlugin() => Task.CompletedTask;
    
    public async Task StartPlugin()
    {
        Observable.Interval(TimeSpan.FromMinutes(TwitterPluginSettings.RevalidatePeriodInMins))
            .Subscribe(_ => RefreshToken())
            .AddTo(_subs);

        EventBus.Receive<TwitterOAuthSuccessEvent>()
            .Subscribe(_ => CacheUserData())
            .AddTo(_subs);

        try
        {
            await RefreshToken();
            await CacheUserData();
        }
        catch (Exception e)
        { Logger.Warning($"Error setting up Twitter Plugin: {e.Message}"); }
    }

    private async Task CacheUserData()
    {
        if (!AppState.HasTwitterOAuth()) { return; }

        try
        {
            var userDetails = await TwitterApiClient.GetCurrentUser();
            AppState.TransientVariables.Set(TwitterVars.UserId, userDetails.Id);
            AppState.TransientVariables.Set(TwitterVars.Username, userDetails.Username);
            AppState.AppVariables.Set(TwitterVars.Username, userDetails.Username);
        }
        catch (Exception e)
        {
            Logger.Warning($"Unable to get user details from twitter: {e.Message}");
            AppState.TransientVariables.Set(TwitterVars.UserId, string.Empty);
            AppState.TransientVariables.Set(TwitterVars.Username, "Unknown User - Missing Permissions");
            AppState.AppVariables.Set(TwitterVars.Username, "Unknown User - Missing Permissions");
        }
    }

    public async Task RefreshToken()
    {
        if (!AppState.HasTwitterOAuth()) { return; }
        await TwitterOAuthClient.RefreshToken();
    }

    public void Dispose()
    { _subs?.Dispose(); }
}