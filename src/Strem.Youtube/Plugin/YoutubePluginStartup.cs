using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Youtube.Events.OAuth;
using Strem.Youtube.Extensions;
using Strem.Youtube.Services.Client;
using Strem.Youtube.Services.OAuth;
using Strem.Youtube.Variables;

namespace Strem.Youtube.Plugin;

public class YoutubePluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public IYoutubeOAuthClient YoutubeOAuthClient { get; }
    public IObservableYoutubeClient YoutubeClient { get; }
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public ILogger<YoutubePluginStartup> Logger { get; }

    public string[] RequiredConfigurationKeys { get; } = Array.Empty<string>();

    public YoutubePluginStartup(IEventBus eventBus, IAppState appState, ILogger<YoutubePluginStartup> logger, IObservableYoutubeClient youtubeClient, IYoutubeOAuthClient youtubeOAuthClient)
    {
        YoutubeOAuthClient = youtubeOAuthClient;
        EventBus = eventBus;
        AppState = appState;
        Logger = logger;
        YoutubeClient = youtubeClient;
    }
    
    public Task SetupPlugin() => Task.CompletedTask;
    
    public async Task StartPlugin()
    {
        Observable.Timer(TimeSpan.FromMinutes(YoutubePluginSettings.RevalidatePeriodInMins))
            .Subscribe(x => VerifyToken())
            .AddTo(_subs);

        EventBus.Receive<YoutubeOAuthSuccessEvent>()
            .Subscribe(_ => CacheUserData())
            .AddTo(_subs);

        try
        {
            await VerifyToken();
            await CacheUserData();
        }
        catch (Exception e)
        { Logger.Warning($"Error setting up Youtube Plugin: {e.Message}"); }
    }

    private async Task CacheUserData()
    {
        if (!AppState.HasYoutubeAccessToken()) { return; }

        try
        {
            var userDetails = await YoutubeOAuthClient.GetUserInfo();
            AppState.TransientVariables.Set(YoutubeVars.UserId, userDetails.Id);
            AppState.TransientVariables.Set(YoutubeVars.Username, userDetails.Name);
            AppState.AppVariables.Set(YoutubeVars.Username, userDetails.Name);
        }
        catch (Exception e)
        {
            Logger.Warning($"Unable to get user details from google/youtube: {e.Message}");
            AppState.TransientVariables.Set(YoutubeVars.UserId, string.Empty);
            AppState.TransientVariables.Set(YoutubeVars.Username, "Unknown User - Missing Permissions");
            AppState.AppVariables.Set(YoutubeVars.Username, "Unknown User - Missing Permissions");
        }
    }

    public async Task VerifyToken()
    {
        Logger.Information("Revalidating Youtube Access Token");

        if (AppState.HasYoutubeAccessToken())
        { await YoutubeOAuthClient.ValidateToken(); }
    }

    public void Dispose()
    { _subs?.Dispose(); }
}