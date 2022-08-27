using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Utils;
using Strem.Core.Variables;

namespace Strem.Flows.Default.Flows.Tasks.Variables;

public class GenerateRandomNumberTask : FlowTask<GenerateRandomNumberTaskData>
{
    public override string Code => GenerateRandomNumberTaskData.TaskCode;
    public override string Version => GenerateRandomNumberTaskData.TaskVersion;
    
    public override string Name => "Generate A Random Number";
    public override string Category => "Variables";
    public override string Description => "Generates a random number between the given range";
    
    public IRandomizer Randomizer { get; }

    public GenerateRandomNumberTask(ILogger<FlowTask<GenerateRandomNumberTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IRandomizer randomizer) : base(logger, flowStringProcessor, appState, eventBus)
    {
        Randomizer = randomizer;
    }

    public override bool CanExecute() => true;

    public override async Task<ExecutionResult> Execute(GenerateRandomNumberTaskData data, IVariables flowVars)
    {
        var processedName = FlowStringProcessor.Process(data.Name, flowVars);
        var processedContext = FlowStringProcessor.Process(data.Context, flowVars);

        if (FlowStringProcessor.TryProcessInt(data.MinimumNumber, flowVars, out var processedMinValue))
        {
            Logger.Warning($"Couldnt process minimum value {data.MinimumNumber}");
            processedMinValue = 0;
        }
        
        if (FlowStringProcessor.TryProcessInt(data.MaximumNumber, flowVars, out var processedMaxValue))
        {
            Logger.Warning($"Couldnt process maximum value {data.MaximumNumber}");
            processedMaxValue = 100;
        }

        var randomValue = Randomizer.Random(processedMinValue, processedMaxValue);
        AppState.SetVariable(flowVars, data.ScopeType, processedName, processedContext, randomValue.ToString());
        return ExecutionResult.Success();
    }
}