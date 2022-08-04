using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Default.Flows.Tasks.Data;

namespace Strem.Flows.Default.Flows.Tasks;

public class ExecuteFlowTask : FlowTask<ExecuteFlowTaskData>
{
    public static readonly string TaskCode = "execute-flow";
    public static readonly string TaskVersion = "1.0.0";
    public override string Code => TaskCode;
    public override string Version => TaskVersion;
    
    public override string Name => "Execute Flow";
    public override string Description => "Executes an existing flow";

    public IFlowExecutionEngine FlowExecutionEngine { get; }
    public IFlowStore FlowStore { get; }

    public ExecuteFlowTask(ILogger<IFlowTask> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IFlowExecutionEngine flowExecutionEngine, IFlowStore flowStore) : base(logger, flowStringProcessor, appState, eventBus)
    {
        FlowExecutionEngine = flowExecutionEngine;
        FlowStore = flowStore;
    }

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