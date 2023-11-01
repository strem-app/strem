using System.Reactive.Disposables;
using System.Reactive.Linq;
using Google.Apis.PeopleService.v1.Data;
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
        
        Observable.Timer(TimeSpan.FromMinutes(YoutubePluginSettings.RefreshChannelPeriodInMins))
            .Subscribe(x => RefreshStreamDetails())
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
            var currentUser = await YoutubeClient.GetCurrentUser();
            var name = currentUser.Names.FirstOrDefault(new Name() { DisplayName = "Unknown User - No Names Listed"}).DisplayName;
            AppState.TransientVariables.Set(YoutubeVars.ChannelId, currentUser.ResourceName);
            AppState.TransientVariables.Set(YoutubeVars.Username, name);
            AppState.AppVariables.Set(YoutubeVars.Username, name);
        }
        catch (Exception e)
        {
            Logger.Warning($"Unable to get user details from google: {e.Message}");
            AppState.TransientVariables.Set(YoutubeVars.ChannelId, string.Empty);
            AppState.TransientVariables.Set(YoutubeVars.Username, "Unknown User - Missing Permissions");
            AppState.AppVariables.Set(YoutubeVars.Username, "Unknown User - Missing Permissions");
        }
    }

    public async Task VerifyToken()
    {
        Logger.Information("Revalidating Youtube Access Token");

        if (AppState.HasYoutubeAccessToken())
        { await YoutubeOAuthClient.RefreshToken(); }
    }

    public async Task RefreshStreamDetails()
    {
        Logger.Information("Updating Youtube Channel Info");
        try
        {
            var usersChannels = await YoutubeClient.GetCurrentUsersChannels();
            foreach (var channel in usersChannels)
            {
                Logger.Information($"User Has Channel: [{channel.Id} | {channel.Snippet.Title}]");
            }
        }
        catch (Exception e)
        {
            Logger.Error($"YT CHANNEL ERROR {e.Message}");
        }
    }

    public void Dispose()
    { _subs?.Dispose(); }
}