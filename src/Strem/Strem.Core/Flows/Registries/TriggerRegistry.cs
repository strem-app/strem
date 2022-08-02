using Strem.Core.Flows.Triggers;

namespace Strem.Core.Flows.Registries;

public class TriggerRegistry : ITriggerRegistry
{
    public List<TriggerDescriptor> Triggers { get; }

    public TriggerRegistry(IEnumerable<TriggerDescriptor> triggers = null)
    {
        Triggers = triggers?.ToList() ?? new List<TriggerDescriptor>();
    }

    public void Add(TriggerDescriptor trigger) => Triggers.Add(trigger);
    public void Remove(TriggerDescriptor trigger) => Triggers.Remove(trigger);
    public TriggerDescriptor Get(string taskCode) => Triggers.SingleOrDefault(x => x.Trigger.Code == taskCode);
    public IEnumerable<TriggerDescriptor> GetAll() => Triggers;
}