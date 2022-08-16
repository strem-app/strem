using Strem.Core.Flows.Triggers;
using Strem.Core.Types;

namespace Strem.Flows.Default.Flows.Triggers.Utility;

public class OnAppStartedTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-app-started";
    public static readonly string TriggerVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;
}