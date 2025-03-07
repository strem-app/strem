using Microsoft.Extensions.Logging;
using Strem.Core.Services.TTS;

namespace Strem.Platforms.OSX.Services.TTS;

public class OSXTTSHandler : ITTSHandler
{
    public ILogger<OSXTTSHandler> Logger { get; }

    public OSXTTSHandler(ILogger<OSXTTSHandler> logger)
    {
        Logger = logger;
    }
    
    public void SayText(TTSRequest request)
    {
        Logger.LogWarning("OSX Platform Does Not Currently Support TTS");
    }
}