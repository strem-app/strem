using Strem.Core.Flows.Tasks;

namespace Strem.Flows.Default.Flows.Tasks.Data;

public class WriteToLogTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "write-to-log";
    public static readonly string TaskVersion = "1.0.0";
    
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string Text { get; set; }
}