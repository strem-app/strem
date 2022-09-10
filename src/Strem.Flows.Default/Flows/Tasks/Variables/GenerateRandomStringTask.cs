using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.Services.Utils;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Executors;
using Strem.Flows.Extensions;
using Strem.Flows.Processors;

namespace Strem.Flows.Default.Flows.Tasks.Variables;

public class GenerateRandomStringTask : FlowTask<GenerateRandomStringTaskData>
{
    public override string Code => GenerateRandomStringTaskData.TaskCode;
    public override string Version => GenerateRandomStringTaskData.TaskVersion;
    
    public override string Name => "Generate A Random String";
    public override string Category => "Variables";
    public override string Description => "Generates a random string of given length";
    
    public IRandomizer Randomizer { get; }

    public GenerateRandomStringTask(ILogger<FlowTask<GenerateRandomStringTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IRandomizer randomizer) : base(logger, flowStringProcessor, appState, eventBus)
    {
        Randomizer = randomizer;
    }

    public override bool CanExecute() => true;

    public override async Task<ExecutionResult> Execute(GenerateRandomStringTaskData data, IVariables flowVars)
    {
        var processedName = FlowStringProcessor.Process(data.Name, flowVars);
        var processedContext = FlowStringProcessor.Process(data.Context, flowVars);

        if (FlowStringProcessor.TryProcessInt(data.StringLength, flowVars, out var processedLength))
        {
            Logger.Warning($"Couldnt process minimum value {data.StringLength}");
            processedLength = 10;
        }
        
        var randomValue = Randomizer.RandomString(processedLength);
        AppState.SetVariable(flowVars, data.ScopeType, processedName, processedContext, randomValue);
        return ExecutionResult.Success();
    }
}