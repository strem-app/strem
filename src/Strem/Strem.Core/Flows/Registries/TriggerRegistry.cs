namespace Strem.Core.Flows.Registries;

public class TriggerRegistry : ITriggerRegistry
{
    public Dictionary<string, TriggerDescriptor> Triggers { get; } = new();
    public void Add(TriggerDescriptor trigger) => Triggers.Add(trigger.Trigger.Code, trigger);
    public void Remove(TriggerDescriptor trigger) => Triggers.Remove(trigger.Trigger.Code);
    public TriggerDescriptor Get(string taskCode) => Triggers.ContainsKey(taskCode) ? Triggers[taskCode] : null;
    public IEnumerable<TriggerDescriptor> GetAll() => Triggers.Values;
}