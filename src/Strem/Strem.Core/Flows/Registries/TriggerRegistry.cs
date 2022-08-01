using Strem.Core.Flows.Triggers;

namespace Strem.Core.Flows.Registries;

public class TriggerRegistry : ITriggerRegistry
{
    public List<IFlowTrigger> Triggers { get; }

    public TriggerRegistry(IEnumerable<IFlowTrigger> triggers = null)
    {
        Triggers = triggers?.ToList() ?? new List<IFlowTrigger>();
    }

    public void Add(IFlowTrigger trigger) => Triggers.Add(trigger);
    public void Remove(IFlowTrigger trigger) => Triggers.Remove(trigger);
    public IFlowTrigger Get(string taskCode) => Triggers.SingleOrDefault(x => x.Code == taskCode);
    public IEnumerable<IFlowTrigger> GetAll() => Triggers;
}