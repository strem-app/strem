using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Services.Audio;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Executors;
using Strem.Flows.Processors;

namespace Strem.Flows.Default.Flows.Tasks.Audio;

public class PlaySoundTask : FlowTask<PlaySoundTaskData>
{
    public override string Code => PlaySoundTaskData.TaskCode;
    public override string Version => PlaySoundTaskData.TaskVersion;
    
    public override string Name => "Play Sound";
    public override string Category => "Audio";
    public override string Description => "Plays a given sound file";

    public IAudioHandler AudioHandler { get; }

    public PlaySoundTask(ILogger<FlowTask<PlaySoundTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IAudioHandler audioHandler) : base(logger, flowStringProcessor, appState, eventBus)
    {
        AudioHandler = audioHandler;
    }

    public override bool CanExecute() => true;

    public override async Task<ExecutionResult> Execute(PlaySoundTaskData data, IVariables flowVars)
    {
        if(!File.Exists(data.SoundFile))
        { return ExecutionResult.Failed($"Sound file cant be found for {data.SoundFile}"); }

        await AudioHandler.PlayFile(data.SoundFile, data.Volume);
        return ExecutionResult.Success();
    }
}