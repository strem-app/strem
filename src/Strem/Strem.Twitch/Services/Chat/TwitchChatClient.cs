using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Twitch.Extensions;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Strem.Twitch.Services.Chat;

public class TwitchChatClient
{
    public IAppState AppState { get; }
    public ILogger<TwitchChatClient> Logger { get; }

    public TwitchChatClient(IAppState appState, ILogger<TwitchChatClient> logger)
    {
        AppState = appState;
        Logger = logger;
    }

    public async Task Connect()
    {
        if (!AppState.HasTwitchOAuth())
        {
            Logger.Error("No access token available, cannot initialize chat");
            return;
        }
        
        var username = AppState.GetTwitchUsername();
        var accessToken = AppState.GetTwitchOAuthToken();
        var credentials = new ConnectionCredentials(username, accessToken);
        var clientOptions = new ClientOptions
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };
        var customClient = new WebSocketClient(clientOptions);
        var client = new TwitchClient(customClient);
        client.Initialize(credentials, username);

        if(!client.Connect())
        { Logger.Error($"Couldnt connect to twitch chat for unknown reason"); }

    }
}