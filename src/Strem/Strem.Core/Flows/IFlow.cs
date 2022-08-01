using Strem.Core.Flows.Tasks;
using Strem.Core.Flows.Triggers;

namespace Strem.Core.Flows;

public class Flow
{
    public string Version { get; set; } = "1.0.0";
    
    public string Name { get; set; }
    public string Category { get; set; }
    
    public List<IFlowTriggerData> TriggerData { get; set; } = new();
    public List<IFlowTaskData> TaskData { get; set; } = new();
}