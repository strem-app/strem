using Microsoft.Extensions.Logging;
using Strem.Core.Services.Audio;

namespace Strem.Platforms.OSX.Services.Audio;

public class OSXAudioHandler : IAudioHandler
{
    public ILogger<OSXAudioHandler> Logger { get; }
    
    public Task PlayFile(string audioFilePath, int volume)
    {
        Logger.LogWarning("OSX Platform Does Not Currently Support Audio Playback");
        return Task.CompletedTask;
    }
}