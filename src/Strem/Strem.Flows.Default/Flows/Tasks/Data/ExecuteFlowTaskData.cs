using Strem.Core.Flows.Tasks;

namespace Strem.Flows.Default.Flows.Tasks.Data;

public class ExecuteFlowTaskData : IFlowTaskData
{
    public string Code => ExecuteFlowTask.TaskCode;
    public string Version { get; set; } = ExecuteFlowTask.TaskVersion;
    
    public string FlowName { get; set; }
    public bool WaitForCompletion { get; set; }
}