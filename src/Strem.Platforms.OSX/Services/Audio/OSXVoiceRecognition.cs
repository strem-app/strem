using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Strem.Core.Models;
using Strem.Core.Services.Audio;

namespace Strem.Platforms.OSX.Services.Audio;

public class OSXVoiceRecognition : IVoiceRecognition
{
    public ILogger<OSXVoiceRecognition> Logger { get; }

    public OSXVoiceRecognition(ILogger<OSXVoiceRecognition> logger)
    {
        Logger = logger;
    }

    public void Dispose() {}

    public IObservable<TermRecognized> ListenForTerm(string term, bool exact)
    {
        Logger.LogWarning("OSX Platform Does Not Currently Support Voice Recognition");
        return Observable.Empty<TermRecognized>();
    }

    public void UpdateTerms(string[] termsToListenFor)
    {}
}