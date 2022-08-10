using Strem.Core.Flows.Tasks;

namespace Strem.Flows.Default.Flows.Tasks.Utility;

public class RaiseEventTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "raise-event";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string EventName { get; set; }
    public string Data { get; set; }
}