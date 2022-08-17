using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Default.Events;
using Strem.Flows.Default.Types;

namespace Strem.Flows.Default.Flows.Tasks.Logic;

public class IfStatementTask : FlowTask<IfStatementTaskData>
{
    public override string Code => IfStatementTaskData.TaskCode;
    public override string Version => IfStatementTaskData.TaskVersion;
    
    public override string Name => "If Statement";
    public override string Category => "Logic";
    public override string Description => "Compares two values";
    
    public IFlowExecutionEngine FlowExecutionEngine { get; }
    public IFlowStore FlowStore { get; }

    public IfStatementTask(ILogger<FlowTask<IfStatementTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IFlowExecutionEngine flowExecutionEngine, IFlowStore flowStore) : base(logger, flowStringProcessor, appState, eventBus)
    {
        FlowExecutionEngine = flowExecutionEngine;
        FlowStore = flowStore;
    }

    public override bool CanExecute() => true;

    public override async Task<bool> Execute(IfStatementTaskData data, IVariables flowVars)
    {
        var doesMatch = data.ComparisonType == ComparisonType.NumericalComparison ? 
            NumericalComparison(data, flowVars) : 
            TextualComparison(data, flowVars);

        if (!doesMatch && data.RunFlowOnFailure && data.FailureFlowId != Guid.Empty)
        {
            var locatedFlow = FlowStore.Flows.SingleOrDefault(x => x.Id == data.FailureFlowId);
            if (locatedFlow != null)
            {
                // This is run without await on purpose
                FlowExecutionEngine?.ExecuteFlow(locatedFlow);
            }
        }

        return doesMatch;
    }

    public bool NumericalComparison(IfStatementTaskData data, IVariables flowVars)
    {
       int numericA, numericB;
       if(!FlowStringProcessor.TryProcessInt(data.ValueA, flowVars, out numericA)) { return false; }
       if(!FlowStringProcessor.TryProcessInt(data.ValueB, flowVars, out numericB)) { return false; }
       return numericA.MatchesOperator(numericB, data.NumberOperator);
    }
    
    public bool TextualComparison(IfStatementTaskData data, IVariables flowVars)
    {
        var processedA = FlowStringProcessor.Process(data.ValueA, flowVars);
        var processedB = FlowStringProcessor.Process(data.ValueB, flowVars);
        return processedA.MatchesText(data.TextOperator, processedB);
    }
}