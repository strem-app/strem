using System.ComponentModel.DataAnnotations;
using Strem.Core.Flows.Tasks;

namespace Strem.Twitch.Flows.Tasks.Chat;

public class SendTwitchChatMessageTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "send-twitch-chat-message";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string Channel { get; set; }
    
    [Required]
    public string Message { get; set; }
}