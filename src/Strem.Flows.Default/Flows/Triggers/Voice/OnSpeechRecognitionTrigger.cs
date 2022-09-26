using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Models;
using Strem.Core.Services.Audio;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Triggers;
using Strem.Flows.Processors;

namespace Strem.Flows.Default.Flows.Triggers.Voice;

public class OnSpeechRecognitionTrigger : FlowTrigger<OnSpeechRecognitionTriggerData>
{
    public override string Code => OnSpeechRecognitionTriggerData.TriggerCode;
    public override string Version => OnSpeechRecognitionTriggerData.TriggerVersion;

    public override string Name => "On Speech Recognition";
    public override string Category => "Voice";
    public override string Description => "Triggers when the matching term is said, this will have to ensure the microphone is always active";
    
    public IVoiceRecognition VoiceRecognition { get; }

    public static VariableEntry TermDataVariable = new("term-matched");
    
    public override VariableDescriptor[] VariableOutputs { get; } = new[] { TermDataVariable.ToDescriptor() };
    
    public OnSpeechRecognitionTrigger(ILogger<FlowTrigger<OnSpeechRecognitionTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IVoiceRecognition voiceRecognition) : base(logger, flowStringProcessor, appState, eventBus)
    {
        VoiceRecognition = voiceRecognition;
    }

    public override bool CanExecute() => OperatingSystem.IsWindows();

    public IVariables PopulateVariables(TermRecognized args)
    {
        var variables = new Core.Variables.Variables();
        variables.Set(TermDataVariable, args.Term);
        return variables;
    }
    
    public override async Task<IObservable<IVariables>> Execute(OnSpeechRecognitionTriggerData data)
    {
        return VoiceRecognition.ListenForTerm(data.TriggerTerm, false)
            //.Where(x => x.Confidence >= data.MinimumConfidence)
            .Select(PopulateVariables);
    }
}