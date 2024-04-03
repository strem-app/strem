using System.Globalization;
using System.Reactive.Linq;
using System.Speech.Recognition;
using Microsoft.Extensions.Logging;
using Strem.Core.Extensions;
using Strem.Core.Models;
using Strem.Core.Services.Audio;

namespace Strem.Platforms.Windows.Services.Audio;

public class VoiceRecognition : IVoiceRecognition
{
    public const string NoiseSink = "noise-sink";
    public const string TermsGrammarName = "terms";
 
    public ILogger<IVoiceRecognition> Logger { get; }

    public SpeechRecognitionEngine SpeechRecognitionEngine { get; private set; }
    public IObservable<SpeechRecognizedEventArgs> OnRecognizedTerm { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsInitialized { get; private set; }
    
    public List<string> KnownPhrases { get; } = new();
    private bool _isAwaitingUpdate;
    private Grammar _termsGrammar;

    public VoiceRecognition(ILogger<IVoiceRecognition> logger)
    {
        Logger = logger;

        try
        {
            // TODO: Not sure how many cultures are supported by this
            SpeechRecognitionEngine = new SpeechRecognitionEngine(CultureInfo.CurrentCulture);
            SpeechRecognitionEngine.SetInputToDefaultAudioDevice();
            
            var noiseSink = new DictationGrammar("grammar:dictation#pronunciation") { Name = NoiseSink };
            SpeechRecognitionEngine.LoadGrammar(noiseSink);

            SpeechRecognitionEngine.RecognizerUpdateReached += OnUpdateGrammer;
        
            OnRecognizedTerm = Observable.FromEventPattern<SpeechRecognizedEventArgs>(
                    x => SpeechRecognitionEngine.SpeechRecognized += x,
                    x => SpeechRecognitionEngine.SpeechRecognized -= x)
                .Select(x => x.EventArgs)
                .Where(ConfirmNotNoiseSink);

            IsInitialized = true;
            Logger.Information($"Speech Recognition has initialized for culture [{CultureInfo.CurrentCulture.Name}], with Recognizer [{SpeechRecognitionEngine.RecognizerInfo.Name} | {SpeechRecognitionEngine.RecognizerInfo.Description}]");
        }
        catch (Exception ex)
        {
            IsInitialized = false;
            Logger.Error($"Unable to initialize Voice Recognition for Culture [{CultureInfo.CurrentCulture.Name}], native error was [{ex.Message}]");
            Logger.Information("This may be due to you not having speech recognition runtimes installed, go to https://www.microsoft.com/en-us/download/details.aspx?id=27224");
        }
    }

    private bool ConfirmNotNoiseSink(SpeechRecognizedEventArgs arg)
    {
        var text = arg.Result.Text;
        return arg.Result.Grammar.Name != NoiseSink;
    }

    public void OnUpdateGrammer(object? sender, RecognizerUpdateReachedEventArgs args)
    {
        if (_termsGrammar != null)
        { SpeechRecognitionEngine.UnloadGrammar(_termsGrammar); }

        var choices = new Choices();
        KnownPhrases.ForEach(x => choices.Add(x));
        var grammarBuilder = new GrammarBuilder(choices) { Culture = CultureInfo.CurrentCulture };
        _termsGrammar = new Grammar(grammarBuilder) { Name = TermsGrammarName };
        SpeechRecognitionEngine.LoadGrammar(_termsGrammar);
        _isAwaitingUpdate = false;
    }
    
    public void UpdateTerms(params string[] termsToListenFor)
    {
        if (!IsActive)
        {
            SpeechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            Logger.Information($"Voice Recognition has been activated, mic is now active");
            IsActive = true;
        }
        
        foreach (var newTerm in termsToListenFor)
        {
            if(!KnownPhrases.Contains(newTerm, StringComparer.OrdinalIgnoreCase))
            { KnownPhrases.Add(newTerm); }
        }

        if(_isAwaitingUpdate) { return; }
        _isAwaitingUpdate = true;
        SpeechRecognitionEngine.RequestRecognizerUpdate();
    }

    public bool MatchesTerm(string text, string term, bool exact)
    {
        if(exact)
        { return text.Equals(term, StringComparison.OrdinalIgnoreCase); }
        return text.Contains(term, StringComparison.OrdinalIgnoreCase);
    }
    
    public IObservable<TermRecognized> ListenForTerm(string term, bool exact)
    {
        if (!IsInitialized)
        {
            Logger.Warning($"Tried to listen for term [{term}], but the speech recognition is not initialized");
            return Observable.Empty<TermRecognized>();
        }
        
        if (!KnownPhrases.Contains(term, StringComparer.OrdinalIgnoreCase))
        { UpdateTerms(term); }
        
        return OnRecognizedTerm
            .Where(x => MatchesTerm(x.Result.Text, term, exact))
            .Select(x => new TermRecognized(x.Result.Text, x.Result.Confidence));
    }

    public void Dispose()
    { SpeechRecognitionEngine?.Dispose(); }
}