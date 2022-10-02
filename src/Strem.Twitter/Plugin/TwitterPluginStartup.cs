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
        Observable.Interval(TimeSpan.FromMinutes(TwitterPluginSettings.RevalidatePeriodInMins))
            .Subscribe(_ => RefreshToken())
            .AddTo(_subs);

        RefreshToken();
    }
    
    public async Task RefreshToken()
    {
        if (!AppState.HasTwitterOAuth()) { return; }
        await TwitterOAuthClient.RefreshToken();
    }

    public void Dispose()
    { _subs?.Dispose(); }
}