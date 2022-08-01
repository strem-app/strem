using Strem.Core.Flows.Tasks;

namespace Strem.Flows.Default.Flows.Tasks.Data;

public class WriteToLogTaskData : IFlowTaskData
{
    public string Code => WriteToLogTask.TaskCode;
    public string Version { get; set; } = WriteToLogTask.TaskVersion;
    
    public string Text { get; set; }
}