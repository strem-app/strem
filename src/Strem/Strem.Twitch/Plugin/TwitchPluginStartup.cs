using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Events;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Twitch.Events;
using Strem.Twitch.Extensions;
using Strem.Twitch.Services.OAuth;

namespace Strem.Twitch.Plugin;

public class TwitchPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs;
    
    public ITwitchOAuthClient TwitchOAuthClient { get; }
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }

    public TwitchPluginStartup(ITwitchOAuthClient twitchOAuthClient, IEventBus eventBus, IAppState appState)
    {
        TwitchOAuthClient = twitchOAuthClient;
        EventBus = eventBus;
        AppState = appState;
    }

    public async Task StartPlugin()
    {
        await TwitchOAuthClient.ValidateToken();

        _subs = new CompositeDisposable();
        Observable.Interval(TimeSpan.FromHours(1))
            .Subscribe(x => VerifyToken())
            .AddTo(_subs);
        
        EventBus.Receive<TwitchOAuthSuccessEvent>()
            .Subscribe(x => VerifyToken())
            .AddTo(_subs);
    }

    public void VerifyToken()
    {
        if (AppState.HasTwitchOAuth())
        { TwitchOAuthClient.ValidateToken(); }
    }

    public void Dispose()
    { _subs?.Dispose(); }
}