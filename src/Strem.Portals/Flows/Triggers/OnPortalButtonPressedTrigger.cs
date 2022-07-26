﻿using System.Reactive.Linq;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Flows.Processors;
using Strem.Flows.Data.Triggers;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Portals.Events;

namespace Strem.Portals.Flows.Triggers;

public class OnPortalButtonPressedTrigger : FlowTrigger<OnPortalButtonPressedTriggerData>
{
    public override string Code => OnPortalButtonPressedTriggerData.TriggerCode;
    public override string Version => OnPortalButtonPressedTriggerData.TriggerVersion;

    public static VariableEntry PortalNameVariable = new("portal-name");
    public static VariableEntry PortalIdVariable = new("portal-id");
    public static VariableEntry ElementNameVariable = new("element-name");
    public static VariableEntry ElementIdVariable = new("element-id");

    public override string Name => "On Portal Button Pressed";
    public override string Category => "Portals";
    public override string Description => "Triggers when a portal button is pressed";
    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        PortalNameVariable.ToDescriptor(), ElementNameVariable.ToDescriptor(),
        PortalIdVariable.ToDescriptor(), ElementIdVariable.ToDescriptor()
    };

    public OnPortalButtonPressedTrigger(ILogger<FlowTrigger<OnPortalButtonPressedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    public override bool CanExecute() => true;

    public bool EventMatchesData(OnPortalButtonPressedTriggerData data, PortalElementPressedEvent eventData)
    {
        if (data.RequiredPortalId != Guid.Empty && data.RequiredPortalId != eventData.PortalId)
        { return false; }

        if (data.RequiredElementId != Guid.Empty && data.RequiredElementId != eventData.ElementId)
        { return false; }

        return true;
    }
    
    public override async Task<IObservable<IVariables>> Execute(OnPortalButtonPressedTriggerData data)
    {
        return EventBus.Receive<PortalElementPressedEvent>()
            .Where(x => EventMatchesData(data, x))
            .Select(x =>
            {
                var newVariables = new Variables();
                newVariables.Set(PortalNameVariable, x.PortalName);
                newVariables.Set(PortalIdVariable, x.PortalId);
                newVariables.Set(ElementNameVariable, x.ButtonName);
                newVariables.Set(ElementIdVariable, x.ElementId);
                return newVariables;
            });
    }
}