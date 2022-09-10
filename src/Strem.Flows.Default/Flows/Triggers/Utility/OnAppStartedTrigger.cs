using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Strem.Core.Events;
using Strem.Core.Events.Bus;
using Strem.Flows.Data.Triggers;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Processors;

namespace Strem.Flows.Default.Flows.Triggers.Utility;

public class OnAppStartedTrigger : FlowTrigger<OnAppStartedTriggerData>
{
    public override string Code => OnAppStartedTriggerData.TriggerCode;
    public override string Version => OnAppStartedTriggerData.TriggerVersion;

    public override string Name => "On App Startup";
    public override string Category => "Utility";
    public override string Description => "Triggers once the app is setup and running";

    public OnAppStartedTrigger(ILogger<FlowTrigger<OnAppStartedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    public override bool CanExecute() => true;

    public override async Task<IObservable<IVariables>> Execute(OnAppStartedTriggerData data)
    {
        return EventBus.Receive<ApplicationStartedEvent>()
            .Select(x => new Core.Variables.Variables());
    }
}