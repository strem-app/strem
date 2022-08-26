using TwitchLib.PubSub.Interfaces;

namespace Strem.Twitch.Services.Client;

public interface IObservableTwitchPubSub
{
    ITwitchPubSub PubSub { get; }
}