using Strem.Flows.Data.Triggers;

namespace Strem.StreamElements.Flows.Triggers;

public class OnStreamElementsTipTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-stream-elements-tip";
    public static readonly string TriggerVersion = "1.0.0";

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;
}