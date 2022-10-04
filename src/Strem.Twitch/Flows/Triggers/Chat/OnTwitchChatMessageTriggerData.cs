using Strem.Flows.Data.Triggers;
using Strem.Core.Types;
using TwitchLib.Client.Enums;

namespace Strem.Twitch.Flows.Triggers.Chat;

public class OnTwitchChatMessageTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-twitch-chat-message";
    public static readonly string TriggerVersion = "1.0.0";

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;
    
    public string? Channel { get; set; }
    public TextMatchType MatchType { get; set; }
    public string MatchText { get; set; } = string.Empty;

    public UserType MinimumUserType { get; set; }
    public bool IsSubscriber { get; set; }
    public bool IsVip { get; set; }
    public bool HasBits { get; set; }
    public bool HasChannelReward { get; set; }
}