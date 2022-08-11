using Strem.Core.Flows.Triggers;
using Strem.Core.Types;
using TwitchLib.Client.Enums;

namespace Strem.Twitch.Flows.Triggers.Chat;

public class OnTwitchChatCommandTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-twitch-chat-command";
    public static readonly string TriggerVersion = "1.0.0";

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;
    
    public string CommandName { get; set; } = string.Empty;

    public UserType MinimumUserType { get; set; }
    public bool RequiresArg { get; set; }
    public bool IsSubscriber { get; set; }
    public bool IsVip { get; set; }
    public bool HasBits { get; set; }
}