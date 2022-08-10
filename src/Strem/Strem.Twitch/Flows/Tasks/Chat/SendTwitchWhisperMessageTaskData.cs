using Strem.Core.Flows.Tasks;

namespace Strem.Twitch.Flows.Tasks.Chat;

public class SendTwitchWhisperMessageTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "send-twitch-whisper-message";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string Username { get; set; }
    public string Message { get; set; }
}