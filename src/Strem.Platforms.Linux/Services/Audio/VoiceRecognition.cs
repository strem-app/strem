using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Strem.Core.Models;
using Strem.Core.Services.Audio;

namespace Strem.Platforms.Linux.Services.Audio;

public class VoiceRecognition : IVoiceRecognition
{
    public ILogger<VoiceRecognition> Logger { get; }

    public VoiceRecognition(ILogger<VoiceRecognition> logger)
    {
        Logger = logger;
    }

    public void Dispose() {}

    public IObservable<TermRecognized> ListenForTerm(string term, bool exact)
    {
        Logger.LogWarning("Linux Platform Does Not Currently Support Voice Recognition");
        return Observable.Empty<TermRecognized>();
    }

    public void UpdateTerms(string[] termsToListenFor)
    {}
}