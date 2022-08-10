using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;

namespace Strem.Flows.Default.Flows.Tasks.Utility;

public class ExecuteFlowTask : FlowTask<ExecuteFlowTaskData>
{
    public override string Code => ExecuteFlowTaskData.TaskCode;
    public override string Version => ExecuteFlowTaskData.TaskVersion;
    
    public override string Name => "Execute Flow";
    public override string Description => "Executes an existing flow";

    public IFlowExecutionEngine FlowExecutionEngine { get; }
    public IFlowStore FlowStore { get; }

    public ExecuteFlowTask(ILogger<FlowTask<ExecuteFlowTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IFlowExecutionEngine flowExecutionEngine, IFlowStore flowStore) : base(logger, flowStringProcessor, appState, eventBus)
    {
        FlowExecutionEngine = flowExecutionEngine;
        FlowStore = flowStore;
    }

    public override bool CanExecute() => true;

    public override async Task Execute(ExecuteFlowTaskData data, IVariables flowVars)
    {
        var flowToExecute = FlowStore.Flows.SingleOrDefault(x => x.Name == data.FlowName);
        if (flowToExecute == null)
        {
            Logger.Warning($"Cant find flow for {data.FlowName} for ExecuteFlowTask");
            return;
        }
        
        if (data.WaitForCompletion)
        { await FlowExecutionEngine.ExecuteFlow(flowToExecute, flowVars); }
        else
        { FlowExecutionEngine.ExecuteFlow(flowToExecute, flowVars); }
    }
}