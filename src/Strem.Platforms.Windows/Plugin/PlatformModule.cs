using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Plugins;
using Strem.Core.Services.Audio;
using Strem.Core.Services.TTS;
using Strem.Platforms.Windows.Services.Audio;
using Strem.Platforms.Windows.Services.TTS;

namespace Strem.Platforms.Windows.Plugin;

public class PlatformModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // TTS
        services.AddSingleton<ITTSHandler, WindowsTTSHandler>();
        
        // Audio
        services.AddSingleton<IAudioHandler, WindowsAudioHandler>();
        services.AddSingleton<IVoiceRecognition, WindowsVoiceRecognition>();
    }
}