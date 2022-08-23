using Strem.Core.Events.Bus;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;
using Strem.Portals.Data.Overrides;

namespace Strem.Portals.Flows.Tasks;

public class ChangePortalButtonStylesTask : FlowTask<ChangePortalButtonStylesTaskData>
{
    public override string Code => ChangePortalButtonStylesTaskData.TaskCode;
    public override string Version => ChangePortalButtonStylesTaskData.TaskVersion;
    
    public override string Name => "Change Portal Button Styles";
    public override string Category => "Portals";
    public override string Description => "Allows you to edit a buttons styles dynamically";

    public ButtonRuntimeStyles ButtonRuntimeStyles { get; }

    public ChangePortalButtonStylesTask(ILogger<FlowTask<ChangePortalButtonStylesTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ButtonRuntimeStyles buttonRuntimeStyles) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ButtonRuntimeStyles = buttonRuntimeStyles;
    }

    public override bool CanExecute() => true;

    public override async Task<ExecutionResult> Execute(ChangePortalButtonStylesTaskData data, IVariables flowVars)
    {
        if (!ButtonRuntimeStyles.RuntimeStyles.ContainsKey(data.PortalId)) { return ExecutionResult.FailedButContinue; }
        if (!ButtonRuntimeStyles.RuntimeStyles[data.PortalId].ContainsKey(data.ButtonId)) { return ExecutionResult.FailedButContinue; }
        return ExecutionResult.Success;
    }
}