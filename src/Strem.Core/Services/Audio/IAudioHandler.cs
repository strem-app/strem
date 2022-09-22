namespace Strem.Core.Services.Audio;

public interface IAudioHandler
{
    Task PlayFile(string audioFilePath, int volume);
}