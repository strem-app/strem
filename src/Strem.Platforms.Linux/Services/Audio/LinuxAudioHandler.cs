using Microsoft.Extensions.Logging;
using Strem.Core.Services.Audio;

namespace Strem.Platforms.Linux.Services.Audio;

public class LinuxAudioHandler : IAudioHandler
{
    public ILogger<LinuxAudioHandler> Logger { get; }
    
    public Task PlayFile(string audioFilePath, int volume)
    {
        Logger.LogWarning("Linux Platform Does Not Currently Support Audio Playback");
        return Task.CompletedTask;
    }
}