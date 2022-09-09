using Strem.Flows.Data.Tasks;

namespace Strem.Flows.Default.Flows.Tasks.Logic;

public class StopFlowTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "stop-flow";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public bool StopParentFlowsToo { get; set; }
}