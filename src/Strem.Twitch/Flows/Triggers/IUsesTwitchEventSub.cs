using Strem.Flows.Data.Triggers;

namespace Strem.Twitch.Flows.Triggers;

public interface IUsesTwitchEventSub
{}

public interface IUsesTwitchEventSub<in T> : IUsesTwitchEventSub
    where T : IFlowTriggerData
{
    /// <summary>
    /// Should setup any eventsub specific subscriptions if needed
    /// </summary>
    /// <param name="data">The payload data describing the trigger</param>
    Task SetupEventSubscriptions(T data);
}