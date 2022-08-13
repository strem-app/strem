using Strem.Core.Flows.Tasks;

namespace Strem.Flows.Default.Flows.Tasks.Utility;

public class WriteToLogTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "write-to-log";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string Text { get; set; }
}