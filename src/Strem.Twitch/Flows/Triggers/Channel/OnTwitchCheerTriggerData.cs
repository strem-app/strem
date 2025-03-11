namespace Strem.Twitch.Flows.Triggers.Channel;

public class OnTwitchCheerTriggerData : ITwitchEventSubTriggerData
{
    public static readonly string TriggerCode = "on-twitch-cheer";
    public static readonly string TriggerVersion = "1.0.0";

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;

    public string RequiredChannel { get; set; }
}