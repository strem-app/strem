using System.ComponentModel.DataAnnotations;
using Strem.Core.Types.TTS;
using Strem.Flows.Data.Tasks;

namespace Strem.Flows.Default.Flows.Tasks.Utility;

public class TextToSpeechTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "text-to-speech";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public string TextToPlay { get; set; }
    [Required]
    public string Volume { get; set; }
    public TTSVoiceGender VoiceGender { get; set; }
    public TTSVoiceAge VoiceAge { get; set; }
}