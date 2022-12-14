using Strem.Core.Events.Bus;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Portals.Data.Overrides;
using Strem.Portals.Events;
using Strem.Portals.Extensions;

namespace Strem.Portals.Flows.Tasks;

public class ChangePortalButtonStylesTask : FlowTask<ChangePortalButtonStylesTaskData>
{
    public override string Code => ChangePortalButtonStylesTaskData.TaskCode;
    public override string Version => ChangePortalButtonStylesTaskData.TaskVersion;
    
    public override string Name => "Change Portal Button Styles";
    public override string Category => "Portals";
    public override string Description => "Allows you to edit a buttons styles dynamically";

    public GridElementRuntimeStyles GridElementRuntimeStyles { get; }

    public ChangePortalButtonStylesTask(ILogger<FlowTask<ChangePortalButtonStylesTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, GridElementRuntimeStyles gridElementRuntimeStyles) : base(logger, flowStringProcessor, appState, eventBus)
    {
        GridElementRuntimeStyles = gridElementRuntimeStyles;
    }

    public override bool CanExecute() => true;

    public override async Task<ExecutionResult> Execute(ChangePortalButtonStylesTaskData data, IVariables flowVars)
    {
        if (!GridElementRuntimeStyles.RuntimeStyles.ContainsKey(data.PortalId)) { return ExecutionResult.FailedButContinue("Cant find portal"); }
        if (!GridElementRuntimeStyles.RuntimeStyles[data.PortalId].ContainsKey(data.ButtonId)) { return ExecutionResult.FailedButContinue("Cant find button"); }
        
        var runtimeStyles = GridElementRuntimeStyles.GetButtonStyles(data.PortalId, data.ButtonId);
        if(data.ChangeButtonType) { runtimeStyles.ButtonType(data.NewStyles.ButtonType()); }
        if(data.ChangeTextColor) { runtimeStyles.ForegroundColor = data.NewStyles.ForegroundColor; }
        if(data.ChangeBackgroundColor) { runtimeStyles.BackgroundColor = data.NewStyles.BackgroundColor; }
        if(data.ChangeIcon) { runtimeStyles.IconClass(data.NewStyles.IconClass()); }
        if(data.ChangeImage) { runtimeStyles.ImageUrl(data.NewStyles.ImageUrl()); }

        if (data.ChangeText)
        {
            var processedText = FlowStringProcessor.Process(data.NewStyles.Text, flowVars);
            runtimeStyles.Text = processedText;
        }
        
        EventBus.PublishAsync(new PortalElementChangedEvent(data.PortalId, data.ButtonId));
        return ExecutionResult.Success();
    }
}