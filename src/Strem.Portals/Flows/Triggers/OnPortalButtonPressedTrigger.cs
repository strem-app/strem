using System.Reactive.Linq;
using Strem.Core.Events.Bus;
using Strem.Core.Events.Portals;
using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Triggers;
using Strem.Core.State;
using Strem.Core.Variables;

namespace Strem.Portals.Flows.Triggers;

public class OnPortalButtonPressedTrigger : FlowTrigger<OnPortalButtonPressedTriggerData>
{
    public override string Code => OnPortalButtonPressedTriggerData.TriggerCode;
    public override string Version => OnPortalButtonPressedTriggerData.TriggerVersion;

    public static VariableEntry PortalNameVariable = new("portal-name");
    public static VariableEntry PortalIdVariable = new("portal-id");
    public static VariableEntry ButtonNameVariable = new("button-name");
    public static VariableEntry ButtonIdVariable = new("button-id");

    public override string Name => "On Portal Button Pressed";
    public override string Category => "Portals";
    public override string Description => "Triggers when a portal button is pressed";
    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        PortalNameVariable.ToDescriptor(), ButtonNameVariable.ToDescriptor(),
        PortalIdVariable.ToDescriptor(), ButtonIdVariable.ToDescriptor()
    };

    public OnPortalButtonPressedTrigger(ILogger<FlowTrigger<OnPortalButtonPressedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    public override bool CanExecute() => true;

    public bool EventMatchesData(OnPortalButtonPressedTriggerData data, ButtonPressedEvent eventData)
    {
        if (data.RequiredPortalId != Guid.Empty && data.RequiredPortalId != eventData.PortalId)
        { return false; }

        if (data.RequiredButtonId != Guid.Empty && data.RequiredButtonId != eventData.ButtonId)
        { return false; }

        return true;
    }
    
    public override IObservable<IVariables> Execute(OnPortalButtonPressedTriggerData data)
    {
        return EventBus.Receive<ButtonPressedEvent>()
            .Where(x => EventMatchesData(data, x))
            .Select(x =>
            {
                var newVariables = new Variables();
                newVariables.Set(PortalNameVariable, x.PortalName);
                newVariables.Set(PortalIdVariable, x.PortalId);
                newVariables.Set(ButtonNameVariable, x.ButtonName);
                newVariables.Set(ButtonIdVariable, x.ButtonId);
                return newVariables;
            });
    }
}