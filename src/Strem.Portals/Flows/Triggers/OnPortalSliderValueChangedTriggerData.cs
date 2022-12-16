using Strem.Core.Services.Validation.Attributes;
using Strem.Flows.Data.Triggers;

namespace Strem.Portals.Flows.Triggers;

public class OnPortalSliderValueChangedTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-portal-slider-value-changed";
    public static readonly string TriggerVersion = "1.0.0";

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;
    
    [NotEmpty]
    public Guid RequiredPortalId { get; set; }
    [NotEmpty]
    public Guid RequiredElementId { get; set; }
}