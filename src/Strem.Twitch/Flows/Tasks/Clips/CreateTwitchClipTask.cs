using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Todos.Data;
using Strem.Todos.Events;
using Strem.Todos.Services.Stores;
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
    
    public static VariableEntry ClipChannelVariable = new("clip.channel", TwitchVars.Context);
    public static VariableEntry ClipUrlVariable = new("clip.url", TwitchVars.Context);
    public static VariableEntry ClipEditUrlVariable = new("clip.url.edit", TwitchVars.Context);
    
    public override string Name => "Create Clip";
    public override string Category => "Twitch";
    public override string Description => "Creates a clip for a given channel, with optional todo entry afterwards";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        ClipUrlVariable.ToDescriptor(), ClipEditUrlVariable.ToDescriptor(), ClipChannelVariable.ToDescriptor()
    };

    public ITwitchAPI TwitchApi { get; }
    public ITodoStore TodoStore { get; }

    public CreateTwitchClipTask(ILogger<FlowTask<CreateTwitchClipTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchAPI twitchApi, ITodoStore todoStore) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchApi = twitchApi;
        TodoStore = todoStore;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.ManageClips);

    public void PopulateVariablesFor(CreateTwitchClipTaskData data, CreatedClipResponse clipResponse, IVariables flowVars)
    {
        if (string.IsNullOrEmpty(data.Channel))
        { flowVars.Set(ClipChannelVariable, AppState.GetTwitchUsername()); }
        else
        {
            var processedChannel = FlowStringProcessor.Process(data.Channel, flowVars);
            { flowVars.Set(ClipChannelVariable, processedChannel); }
        }
        
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
            Id = Guid.NewGuid(),
            Payload = clip.CreatedClips[0].EditUrl,
            ActionType = TodoActionType.Link,
            Title = title,
            CreatedDate = DateTime.Now,
            ExpiryDate = DateTime.Now.AddDays(1),
            CreatedBy = "Twitch Clip Task",
            Tags = data.Tags
        };
        TodoStore.Add(todoElement);
    }
    
    public override async Task<ExecutionResult> Execute(CreateTwitchClipTaskData data, IVariables flowVars)
    {
        var userId = AppState.GetTwitchUserId();
        if (!string.IsNullOrEmpty(data.Channel))
        {
            var processedChannel = FlowStringProcessor.Process(data.Channel, flowVars);
            var userDetails = await TwitchApi.Helix.Users.GetUsersAsync(logins: new () { processedChannel });
            if (userDetails.Users.Length == 0)
            {
                Logger.Warning($"Couldnt Find UserId For Use [{ processedChannel }]");
                return ExecutionResult.Failed($"Couldnt Find UserId For Use [{ processedChannel }]");
            }

            userId = userDetails.Users[0].Id;
        }
        
        var clip = await TwitchApi.Helix.Clips.CreateClipAsync(userId);
        if(clip?.CreatedClips.Length == 0) { return ExecutionResult.Failed("Twitch couldnt create clip for some reason"); }

        PopulateVariablesFor(data, clip, flowVars);
        CreateTodoIfNeeded(data, clip);
        
        return ExecutionResult.Success();
    }
}