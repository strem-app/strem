using NAudio.Wave;
using Strem.Core.Services.Audio;

namespace Strem.Platforms.Windows.Services.Audio;

public class WindowsAudioHandler : IAudioHandler
{
    public async Task PlayFile(string audioFilePath, int volume)
    {
        var trueVolume = (float)volume / 100;
        using var audioFile = new MediaFoundationReader(audioFilePath);
        using var outputDevice = new WaveOutEvent();
        
        outputDevice.Init(audioFile);
        outputDevice.Volume = trueVolume;
        outputDevice.Play();
        while (outputDevice.PlaybackState == PlaybackState.Playing)
        {
            await Task.Delay(1000);
        }
    }
}