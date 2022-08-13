using Strem.Core.Flows.Tasks;
using Strem.Core.Flows.Triggers;

namespace Strem.Core.Flows;

public class Flow
{
    public string Version { get; set; } = "1.0.0";
    
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public bool Enabled { get; set; }
    
    public List<IFlowTriggerData> TriggerData { get; set; } = new();
    public List<IFlowTaskData> TaskData { get; set; } = new();

    public Flow(Guid id, string name, string category)
    {
        Id = id;
        Name = name;
        Category = category;
    }

    public Flow()
    {
    }
}