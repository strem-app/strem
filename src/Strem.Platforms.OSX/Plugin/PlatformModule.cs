using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Plugins;
using Strem.Core.Services.Audio;
using Strem.Core.Services.TTS;
using Strem.Platforms.OSX.Services.Audio;
using Strem.Platforms.OSX.Services.TTS;

namespace Strem.Platforms.OSX.Plugin;

public class PlatformModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // TTS
        services.AddSingleton<ITTSHandler, OSXTTSHandler>();
        
        // Audio
        services.AddSingleton<IAudioHandler, OSXAudioHandler>();
        services.AddSingleton<IVoiceRecognition, OSXVoiceRecognition>();
    }
}