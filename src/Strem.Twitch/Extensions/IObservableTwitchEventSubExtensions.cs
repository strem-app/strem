using Strem.Twitch.Services.Client;

namespace Strem.Twitch.Extensions;

public static class IObservableTwitchEventSubExtensions
{
    public static async Task<bool> SubscribeOnChannelIfNeeded(this IObservableTwitchEventSub observableTwitchEventSub, string subType, string channelName, string version = "1")
    {
        if (observableTwitchEventSub.HasSubscribedTo(subType, channelName))
        { return true; }

        return await observableTwitchEventSub.SubscribeOnChannel(subType, channelName);
    }
}