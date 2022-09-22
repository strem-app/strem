using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Services.TTS;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Extensions;

namespace Strem.Flows.Default.Flows.Tasks.Utility;

public class TextToSpeechTask : FlowTask<TextToSpeechTaskData>
{
    public override string Code => TextToSpeechTaskData.TaskCode;
    public override string Version => TextToSpeechTaskData.TaskVersion;
    
    public override string Name => "Text To Speech";
    public override string Category => "Utility";
    public override string Description => "Makes the computer speak out the text provided";

    public IFlowExecutor FlowExecutor { get; }
    public ITTSHandler TTSHandler { get; }

    public TextToSpeechTask(ILogger<FlowTask<TextToSpeechTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IFlowExecutor flowExecutor, ITTSHandler ttsHandler) : base(logger, flowStringProcessor, appState, eventBus)
    {
        FlowExecutor = flowExecutor;
        TTSHandler = ttsHandler;
    }

    public override bool CanExecute() => true;

    public override async Task<ExecutionResult> Execute(TextToSpeechTaskData data, IVariables flowVars)
    {
        var textToSay = FlowStringProcessor.Process(data.TextToPlay, flowVars);
        if(string.IsNullOrEmpty(textToSay))
        { return ExecutionResult.Failed("No text provided to say"); }

        if (!FlowStringProcessor.TryProcessInt(data.Volume, flowVars, out var processedVolume))
        { processedVolume = 100; }

        var request = new TTSRequest
        {
            Volume = processedVolume,
            VoiceGender = data.VoiceGender,
            VoiceAge = data.VoiceAge,
            TextToSay = textToSay
        };
        TTSHandler.SayText(request);
        
        return ExecutionResult.Success();
    }
}