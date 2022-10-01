using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Twitter.Events.OAuth;
using Strem.Twitter.Extensions;
using Strem.Twitter.Services.OAuth;
using Strem.Twitter.Variables;
using Tweetinvi;
using Tweetinvi.Client;
using Tweetinvi.Models;

namespace Strem.Twitter.Plugin;

public class TwitterPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public ITwitterOAuthClient TwitterOAuthClient { get; }
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public ILogger<TwitterPluginStartup> Logger { get; }

    public string[] RequiredConfigurationKeys { get; } = new[] { TwitterPluginSettings.TwitterClientIdKey };

    public TwitterPluginStartup(ITwitterOAuthClient twitterOAuthClient, IEventBus eventBus, IAppState appState, ILogger<TwitterPluginStartup> logger)
    {
        TwitterOAuthClient = twitterOAuthClient;
        EventBus = eventBus;
        AppState = appState;
        Logger = logger;
    }

    public Task SetupPlugin() => Task.CompletedTask;
    
    public async Task StartPlugin()
    {
        EventBus.Receive<TwitterOAuthSuccessEvent>()
            .Subscribe(x => SetupConnections())
            .AddTo(_subs);
        
        EventBus.Receive<TwitterOAuthRevokedEvent>()
            .Subscribe(x => DisconnectEverything())
            .AddTo(_subs);

        Observable.Interval(TimeSpan.FromMinutes(TwitterPluginSettings.RevalidatePeriodInMins))
            .Subscribe(_ => RefreshToken())
            .AddTo(_subs);

        RefreshToken();
    }

    public async Task SetupConnections()
    {
        var twitterCredentials = new TwitterCredentials() { AccessToken = AppState.GetTwitterOAuthToken() };
        var twitterApi = new TwitterClient(twitterCredentials);
        var user = await twitterApi.Users.GetAuthenticatedUserAsync();
        AppState.AppVariables.Set(TwitterVars.Username, user.Name);
    }

    public async Task DisconnectEverything()
    {

    }

    public async Task RefreshToken()
    {
        if (!AppState.HasTwitterOAuth()) { return; }
       
        var hasRefreshed = await TwitterOAuthClient.RefreshToken();
        if(!hasRefreshed) { return; }

        await SetupConnections();
    }

    public void Dispose()
    { _subs?.Dispose(); }
}