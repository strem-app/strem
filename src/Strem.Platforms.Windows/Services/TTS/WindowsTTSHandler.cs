using System.Speech.Synthesis;
using Strem.Core.Services.TTS;

namespace Strem.Platforms.Windows.Services.TTS;

public class WindowsTTSHandler : ITTSHandler, IDisposable
{
    public SpeechSynthesizer SpeechSynthesizer { get; }

    public WindowsTTSHandler()
    {
        SpeechSynthesizer = new SpeechSynthesizer();
    }

    public void SayText(TTSRequest request)
    {
        SpeechSynthesizer.Volume = request.Volume;
        SpeechSynthesizer.SelectVoiceByHints((VoiceGender)request.VoiceGender, (VoiceAge)request.VoiceAge);
        SpeechSynthesizer.SpeakAsync(request.TextToSay);
    }

    public void Dispose()
    {
        SpeechSynthesizer.Dispose();
    }
}