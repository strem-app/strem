using Strem.Core.Flows.Triggers;

namespace Strem.Twitch.Flows.Triggers.Chat;

public class OnTwitchGiftSubTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-twitch-gift-sub";
    public static readonly string TriggerVersion = "1.0.0";

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;

    public string RequiredChannel { get; set; }
}