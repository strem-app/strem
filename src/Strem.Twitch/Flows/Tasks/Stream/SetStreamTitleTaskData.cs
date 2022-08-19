using Strem.Core.Flows.Tasks;

namespace Strem.Twitch.Flows.Tasks.Stream;

public class SetStreamTitleTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "set-twitch-stream-title";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string Title { get; set; }
}