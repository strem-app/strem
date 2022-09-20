using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Services.Utils;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
namespace Strem.Flows.Default.Flows.Tasks.Logic;

public class RandomSubFlowTask : FlowTask<RandomSubFlowTaskData>
{
    public override string Code => RandomSubFlowTaskData.TaskCode;
    public override string Version => RandomSubFlowTaskData.TaskVersion;
    
    public override string Name => "Random Sub-Flow";
    public override string Category => "Logic";
    public override string Description => "Will randomly pick one of the given sub flows to excute";
    
    public IRandomizer Randomizer { get; }

    public RandomSubFlowTask(ILogger<FlowTask<RandomSubFlowTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IRandomizer randomizer) : base(logger, flowStringProcessor, appState, eventBus)
    {
        Randomizer = randomizer;
    }

    public override bool CanExecute() => true;
    
    public override async Task<ExecutionResult> Execute(RandomSubFlowTaskData data, IVariables flowVars)
    {
        if(data.SubFlowNames.Count == 0)
        { return ExecutionResult.FailedButContinue("There were no available sub flows to choose"); }
        
        var randomSubTask = Randomizer.PickRandomFrom(data.SubFlowNames);
        return ExecutionResult.Success(randomSubTask);
    }
}