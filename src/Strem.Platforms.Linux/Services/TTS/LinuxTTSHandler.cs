using Microsoft.Extensions.Logging;
using Strem.Core.Services.TTS;

namespace Strem.Platforms.Linux.Services.TTS;

public class LinuxTTSHandler : ITTSHandler
{
    public ILogger<LinuxTTSHandler> Logger { get; }

    public LinuxTTSHandler(ILogger<LinuxTTSHandler> logger)
    {
        Logger = logger;
    }
    
    public void SayText(TTSRequest request)
    {
        Logger.LogWarning("Linux Platform Does Not Currently Support TTS");
    }
}