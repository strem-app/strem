using Strem.Flows.Data.Triggers;

namespace Strem.Flows.Services.Data.Cloners;

public interface IFlowTriggerCloner
{
    public IFlowTriggerData Clone(IFlowTriggerData triggerToClone);
}