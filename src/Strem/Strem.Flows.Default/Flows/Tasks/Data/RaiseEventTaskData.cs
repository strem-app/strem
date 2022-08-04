using Strem.Core.Flows.Tasks;
using Strem.Core.Types;

namespace Strem.Flows.Default.Flows.Tasks.Data;

public class RaiseEventTaskData : IFlowTaskData
{
    public string Code => RaiseEventTask.TaskCode;
    public string Version { get; set; } = RaiseEventTask.TaskVersion;
    
    public string EventName { get; set; }
    public string Data { get; set; }
}