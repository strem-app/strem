namespace Strem.Core.Flows.Triggers;

public interface IFlowTriggerData
{
    string Version { get; }
    string TriggerCode { get; }
}