using InputSimulatorStandard;
using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Plugins;
using Strem.Core.Services.Audio;
using Strem.Core.Services.Browsers.File;
using Strem.Core.Services.Clipboard;
using Strem.Core.Services.TTS;
using Strem.Platforms.Windows.Services.Audio;
using Strem.Platforms.Windows.Services.Browsers;
using Strem.Platforms.Windows.Services.Clipboard;
using Strem.Platforms.Windows.Services.TTS;

namespace Strem.Platforms.Windows.Plugin;

public class WindowsPlatformModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // File Browsing
        services.AddSingleton<IFileBrowser, FileBrowser>();
        
        // TTS
        services.AddSingleton<ITTSHandler, WindowsTTSHandler>();
        
        // Clipboard
        services.AddSingleton<IClipboardHandler, ClipboardHandler>();
        
        // Audio
        services.AddSingleton<IAudioHandler, WindowsAudioHandler>();
        services.AddSingleton<IVoiceRecognition, VoiceRecognition>();
    }
}