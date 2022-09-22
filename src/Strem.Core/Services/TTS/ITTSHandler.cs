namespace Strem.Core.Services.TTS;

public interface ITTSHandler
{
    void SayText(TTSRequest request);
}