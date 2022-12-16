using System.Reactive.Linq;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Triggers;
using Strem.Flows.Processors;
using Strem.Portals.Events;

namespace Strem.Portals.Flows.Triggers;

public class OnPortalSliderValueChangedTrigger : FlowTrigger<OnPortalSliderValueChangedTriggerData>
{
    public override string Code => OnPortalSliderValueChangedTriggerData.TriggerCode;
    public override string Version => OnPortalSliderValueChangedTriggerData.TriggerVersion;

    public static VariableEntry PortalNameVariable = new("portal-name");
    public static VariableEntry PortalIdVariable = new("portal-id");
    public static VariableEntry ElementNameVariable = new("element-name");
    public static VariableEntry ElementIdVariable = new("element-id");
    public static VariableEntry SliderValueVariable = new("slider-value");

    public override string Name => "On Portal Slider Value Changed";
    public override string Category => "Portals";
    public override string Description => "Triggers when a portal slider has its value changed";
    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        PortalNameVariable.ToDescriptor(), ElementNameVariable.ToDescriptor(),
        PortalIdVariable.ToDescriptor(), ElementIdVariable.ToDescriptor(),
        SliderValueVariable.ToDescriptor()
    };

    public OnPortalSliderValueChangedTrigger(ILogger<FlowTrigger<OnPortalSliderValueChangedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    public override bool CanExecute() => true;

    public bool EventMatchesData(OnPortalSliderValueChangedTriggerData data, PortalSliderValueChangedEvent eventData)
    {
        if (data.RequiredPortalId != Guid.Empty && data.RequiredPortalId != eventData.PortalId)
        { return false; }

        if (data.RequiredElementId != Guid.Empty && data.RequiredElementId != eventData.ElementId)
        { return false; }

        return true;
    }
    
    public override async Task<IObservable<IVariables>> Execute(OnPortalSliderValueChangedTriggerData data)
    {
        return EventBus.Receive<PortalSliderValueChangedEvent>()
            .Where(x => EventMatchesData(data, x))
            .Select(x =>
            {
                var newVariables = new Variables();
                newVariables.Set(PortalNameVariable, x.PortalName);
                newVariables.Set(PortalIdVariable, x.PortalId);
                newVariables.Set(ElementNameVariable, x.ElementName);
                newVariables.Set(ElementIdVariable, x.ElementId);
                newVariables.Set(SliderValueVariable, x.Value);
                return newVariables;
            });
    }
}