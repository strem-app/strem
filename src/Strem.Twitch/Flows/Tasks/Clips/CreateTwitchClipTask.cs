using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Todos.Data;
using Strem.Todos.Events;
using Strem.Twitch.Extensions;
using Strem.Twitch.Types;
using Strem.Twitch.Variables;
using TwitchLib.Api.Helix;
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

    public CreateTwitchClipTask(ILogger<FlowTask<CreateTwitchClipTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchAPI twitchApi, ITodoStore todoStore) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchApi = twitchApi;
        TodoStore = todoStore;
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
            Id = Guid.NewGuid(),
            Payload = clip.CreatedClips[0].EditUrl,
            ActionType = TodoActionType.Link,
            Title = title,
            CreatedDate = DateTime.Now,
            ExpiryDate = DateTime.Now.AddDays(1),
            CreatedBy = "Twitch Clip Task",
            Tags = data.Tags
        };
        TodoStore.Todos.Add(todoElement);
        EventBus.PublishAsync(new TodoCreatedEvent { TodoId = todoElement.Id });
    }
    
    public override async Task<ExecutionResult> Execute(CreateTwitchClipTaskData data, IVariables flowVars)
    {
        var userId = AppState.GetTwitchUserId();
        if (!string.IsNullOrEmpty(data.Channel))
        {
            var userDetails = await TwitchApi.Helix.Users.GetUsersAsync(logins: new () { data.Channel });
            if (userDetails.Users.Length == 0)
            {
                Logger.Warning($"Couldnt Find UserId For Use [{ data.Channel }]");
                return ExecutionResult.Failed($"Couldnt Find UserId For Use [{ data.Channel }]");
            }

            userId = userDetails.Users[0].Id;
        }
        
        
        
        var clip = await TwitchApi.Helix.Clips.CreateClipAsync(userId);
        if(clip?.CreatedClips.Length == 0) { return ExecutionResult.Failed("Twitch couldnt create clip for some reason"); }

        PopulateVariablesFor(clip, flowVars);
        CreateTodoIfNeeded(data, clip);
        
        return ExecutionResult.Success();
    }
}