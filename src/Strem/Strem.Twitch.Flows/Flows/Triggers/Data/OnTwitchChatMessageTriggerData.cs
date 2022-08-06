using Strem.Core.Flows.Triggers;
using Strem.Core.Types;
using TwitchLib.Api.Core.Enums;

namespace Strem.Twitch.Flows.Flows.Triggers.Data;

public class OnTwitchChatMessageTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-twitch-chat-message";
    public static readonly string TriggerVersion = "1.0.0";

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;
    
    public TextMatch MatchType { get; set; }
    public string MatchText { get; set; } = string.Empty;

    public UserType MinimumUserType { get; set; }
    public bool IsSubscriber { get; set; }
    public bool HasBits { get; set; }
    public bool HasChannelReward { get; set; }
}