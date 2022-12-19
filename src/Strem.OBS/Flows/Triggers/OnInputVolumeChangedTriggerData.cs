using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Triggers;

namespace Strem.OBS.Flows.Triggers;

public class OnInputVolumeChangedTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-obs-input-volume-changed";
    public static readonly string TriggerVersion = "1.0.0";

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;

    [Required]
    public string InputName { get; set; }
    
    public bool TriggerOnStart { get; set; }
}