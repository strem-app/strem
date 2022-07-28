using Strem.Twitch.Models;

namespace Strem.Twitch.Events;

public class TwitchOAuthedEvent
{
    public TwitchOAuthClientPayload OAuthData { get; set; }

    public TwitchOAuthedEvent(TwitchOAuthClientPayload oAuthData)
    {
        OAuthData = oAuthData;
    }
}