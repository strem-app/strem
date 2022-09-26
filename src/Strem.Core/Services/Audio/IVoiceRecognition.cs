using Strem.Core.Models;

namespace Strem.Core.Services.Audio;

public interface IVoiceRecognition : IDisposable
{
    IObservable<TermRecognized> ListenForTerm(string term, bool exact);
    void UpdateTerms(string[] termsToListenFor);
}