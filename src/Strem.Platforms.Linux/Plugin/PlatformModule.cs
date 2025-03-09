using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Plugins;
using Strem.Core.Services.Audio;
using Strem.Core.Services.TTS;
using Strem.Platforms.Linux.Services.Audio;
using Strem.Platforms.Linux.Services.TTS;

namespace Strem.Platforms.Linux.Plugin;

public class PlatformModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // TTS
        services.AddSingleton<ITTSHandler, LinuxTTSHandler>();
        
        // Audio
        services.AddSingleton<IAudioHandler, LinuxAudioHandler>();
        services.AddSingleton<IVoiceRecognition, VoiceRecognition>();
    }
}