using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Triggers;

namespace Strem.Flows.Default.Flows.Triggers.Voice;

public class OnSpeechRecognitionTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-speech-recognition";
    public static readonly string TriggerVersion = "1.0.0";

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;
    
    [Required]
    public string TriggerTerm { get; set; } = string.Empty;
    public float MinimumConfidence { get; set; } = 0.8f;
}