using Strem.Core.Events.Bus;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Executors;
using Strem.Flows.Extensions;
using Strem.Flows.Processors;
using Strem.Portals.Data.Overrides;
using Strem.Portals.Events;
using Strem.Portals.Extensions;
using Strem.Portals.Services.Stores;
using Strem.Portals.Types;

namespace Strem.Portals.Flows.Tasks;

public class SetPortalSliderValueTask : FlowTask<SetPortalSliderValueTaskData>
{
    public override string Code => SetPortalSliderValueTaskData.TaskCode;
    public override string Version => SetPortalSliderValueTaskData.TaskVersion;
    
    public override string Name => "Set Portal Slider Value";
    public override string Category => "Portals";
    public override string Description => "Allows you to set a portals slider element value dynamically";

    public IPortalStore PortalStore { get; }

    public SetPortalSliderValueTask(ILogger<FlowTask<SetPortalSliderValueTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IPortalStore portalStore) : base(logger, flowStringProcessor, appState, eventBus)
    {
        PortalStore = portalStore;
    }

    public override bool CanExecute() => true;

    public override async Task<ExecutionResult> Execute(SetPortalSliderValueTaskData data, IVariables flowVars)
    {
        var portalData = PortalStore.Get(data.PortalId);
        if(portalData == null) { return ExecutionResult.Failed("Portal cant be found"); }
        var sliderElement = portalData.Elements.SingleOrDefault(x => x.Id == data.ElementId && x.ElementType == GridElementType.Slider);
        if(sliderElement == null) { return ExecutionResult.Failed("The slider cant be found within this portal"); }

        if(!FlowStringProcessor.TryProcessInt(data.NewValue, flowVars, out var processedValue))
        { return ExecutionResult.Failed($"Cannot process value [{data.NewValue}] to number"); }
        
        sliderElement.CurrentValue(processedValue);
        EventBus.PublishAsync(new PortalElementChangedEvent(data.PortalId, data.ElementId));
        return ExecutionResult.Success();
    }
}