using Strem.Flows.Data.Triggers;

namespace Strem.Flows.Services.Registries.Triggers;

public class TriggerDescriptor
{
    public IFlowTrigger Trigger { get; }
    public Func<IFlowTriggerData> Data { get; }
    public Type ComponentType { get; }

    public TriggerDescriptor(IFlowTrigger trigger, Func<IFlowTriggerData> data, Type componentType)
    {
        Trigger = trigger;
        Data = data;
        ComponentType = componentType;
    }
}