using Strem.Core.Services.Utils;
using Strem.Flows.Data.Triggers;

namespace Strem.Flows.Services.Data.Cloners;

public class FlowTriggerCloner : IFlowTriggerCloner
{
    public ICloner Cloner { get; }

    public FlowTriggerCloner(ICloner cloner)
    {
        Cloner = cloner;
    }

    public IFlowTriggerData Clone(IFlowTriggerData triggerToClone)
    {
        var clonedTrigger = Cloner.Clone(triggerToClone);
        clonedTrigger.Id = Guid.NewGuid();
        return clonedTrigger;
    }
}