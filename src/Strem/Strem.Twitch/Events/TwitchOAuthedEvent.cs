using Strem.Twitch.Models;

namespace Strem.Twitch.Events;

public class TwitchOAuthedEvent
{
    public TwitchOAuthResponse OAuthData { get; set; }

    public TwitchOAuthedEvent(TwitchOAuthResponse oAuthData)
    {
        OAuthData = oAuthData;
    }
}