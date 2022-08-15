using Strem.Core.Flows.Triggers;
using Strem.Core.Types;

namespace Strem.Twitch.Flows.Triggers.Chat;

public class OnTwitchWhisperMessageTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-twitch-whisper-message";
    public static readonly string TriggerVersion = "1.0.0";

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;
    
    public TextMatchType MatchTypeType { get; set; }
    public string MatchText { get; set; } = string.Empty;
}