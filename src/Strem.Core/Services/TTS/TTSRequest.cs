using Strem.Core.Types.TTS;

namespace Strem.Core.Services.TTS;

public class TTSRequest
{
    public int Volume { get; set; } = 100;
    public TTSVoiceGender VoiceGender { get; set; } = TTSVoiceGender.NotSet;
    public TTSVoiceAge VoiceAge { get; set; } = TTSVoiceAge.NotSet;
    public string TextToSay { get; set; } = string.Empty;
}