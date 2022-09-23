namespace Strem.Twitch.Plugin;

public class TwitchPluginSettings
{
    // Local Config
    public static readonly int RevalidatePeriodInMins = 60;
    public static readonly int ChatReconnectInMins = 1;
    public static readonly int RefreshChannelPeriodInMins = 5;
    
    // App Config
    public static readonly string TwitchClientIdKey = "twitch-client-id";
}