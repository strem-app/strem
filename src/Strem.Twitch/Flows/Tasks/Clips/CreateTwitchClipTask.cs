using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Todo;
using Strem.Core.Variables;
using Strem.Twitch.Extensions;
using Strem.Twitch.Types;
using Strem.Twitch.Variables;
using TwitchLib.Api.Helix.Models.Clips.CreateClip;
using TwitchLib.Api.Interfaces;

namespace Strem.Twitch.Flows.Tasks.Clips;

public class CreateTwitchClipTask : FlowTask<CreateTwitchClipTaskData>
{
    public override string Code => CreateTwitchClipTaskData.TaskCode;
    public override string Version => CreateTwitchClipTaskData.TaskVersion;
    
    public static VariableEntry ClipUrlVariable = new("clip.url", TwitchVars.TwitchContext);
    public static VariableEntry ClipEditUrlVariable = new("clip.url.edit", TwitchVars.TwitchContext);
    
    public override string Name => "Create Clip";
    public override string Category => "Twitch";
    public override string Description => "Creates a clip for a given channel, with optional todo entry afterwards";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        ClipUrlVariable.ToDescriptor(), ClipEditUrlVariable.ToDescriptor()
    };

    public ITwitchAPI TwitchApi { get; }
    public ITodoStore TodoStore { get; }

    public CreateTwitchClipTask(ILogger<FlowTask<CreateTwitchClipTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchAPI twitchApi) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchApi = twitchApi;
    }

    public override bool CanExecute() => AppState.HasTwitchOAuth() && AppState.HasTwitchScope(ApiScopes.ManageClips);

    public void PopulateVariablesFor(CreatedClipResponse clipResponse, IVariables flowVars)
    {
        var clip = clipResponse.CreatedClips[0];
        flowVars.Set(ClipEditUrlVariable, clip.EditUrl);
        flowVars.Set(ClipUrlVariable, clip.EditUrl.Replace("/Edit", "", StringComparison.OrdinalIgnoreCase));
    }
    
    private void CreateTodoIfNeeded(CreateTwitchClipTaskData data, CreatedClipResponse clip)
    {
        if(!data.CreateTodo) { return; }

        var truncatedChannelTitle = AppState.TransientVariables.Get(TwitchVars.ChannelTitle).Truncate(20);
        var title = $"Twitch Clip Needs Editing - {truncatedChannelTitle}";
        var todoElement = new TodoData
        {
            Payload = clip.CreatedClips[0].EditUrl,
            ActionType = TodoActionType.Link,
            Title = title,
            CreatedDate = DateTime.Now,
            ExpiryDate = DateTime.Now.AddDays(1),
            CreatedBy = "Twitch Clip Task"
        };
        TodoStore.Todos.Add(todoElement);
    }
    
    public override async Task<bool> Execute(CreateTwitchClipTaskData data, IVariables flowVars)
    {
        var channel = string.IsNullOrEmpty(data.Channel) ? AppState.GetTwitchUsername() : data.Channel;
        var clip = await TwitchApi.Helix.Clips.CreateClipAsync(channel);
        if(clip?.CreatedClips.Length == 0)
        { return false; }

        PopulateVariablesFor(clip, flowVars);
        CreateTodoIfNeeded(data, clip);
        
        return true;
    }
}