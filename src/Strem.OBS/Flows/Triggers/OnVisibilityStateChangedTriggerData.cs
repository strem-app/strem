using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Triggers;

namespace Strem.OBS.Flows.Triggers;

public class OnVisibilityStateChangedTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-obs-visibility-state-changed";
    public static readonly string TriggerVersion = "1.0.0";

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;
    
    [Required]
    public string SceneName { get; set; }
    
    [Required]
    public string SceneItemName { get; set; }
    
    public bool TriggerOnVisible { get; set; }
    public bool TriggerOnInvisible { get; set; }
    public bool TriggerOnStart { get; set; }
}