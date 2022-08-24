using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Default.Types;

namespace Strem.Flows.Default.Flows.Tasks.Logic;

public class IfStatementTask : FlowTask<IfStatementTaskData>
{
    public override string Code => IfStatementTaskData.TaskCode;
    public override string Version => IfStatementTaskData.TaskVersion;
    
    public override string Name => "If Statement";
    public override string Category => "Logic";
    public override string Description => "Compares two values";
    
    public IFlowExecutor FlowExecutor { get; }

    public IfStatementTask(ILogger<FlowTask<IfStatementTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IFlowExecutor flowExecutor) : base(logger, flowStringProcessor, appState, eventBus)
    {
        FlowExecutor = flowExecutor;
    }

    public override bool CanExecute() => true;
    
    public override async Task<ExecutionResult> Execute(IfStatementTaskData data, IVariables flowVars)
    {
        var doesMatch = data.ComparisonType == ComparisonType.NumericalComparison ? 
            NumericalComparison(data, flowVars) : 
            TextualComparison(data, flowVars);

        var activatedSubTasks = doesMatch ? IfStatementTaskData.TrueSubTaskKey : IfStatementTaskData.FalseSubTaskKey;
        return ExecutionResult.Success(activatedSubTasks);
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