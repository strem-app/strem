using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Services.Execution;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Executors;
using Strem.Flows.Extensions;
using Strem.Flows.Processors;
using Strem.Flows.Types;

namespace Strem.Flows.Default.Flows.Tasks.Utility;

public class ExclusiveExecutionTask : FlowTask<ExclusiveExecutionTaskData>, INotifyOnSubTasksFinished
{
    public override string Code => ExclusiveExecutionTaskData.TaskCode;
    public override string Version => ExclusiveExecutionTaskData.TaskVersion;
    
    public override string Name => "Exclusive Execution";
    public override string Category => "Utility";
    public override string Description => "Allows exclusive execution against a given group name";
    
    public IExclusiveExecutionHandler ExclusiveExecutionHandler { get; }

    public ExclusiveExecutionTask(ILogger<FlowTask<ExclusiveExecutionTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IExclusiveExecutionHandler exclusiveExecutionHandler) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ExclusiveExecutionHandler = exclusiveExecutionHandler;
    }

    public override bool CanExecute() => true;
    
    public override async Task<ExecutionResult> Execute(ExclusiveExecutionTaskData data, IVariables flowVars)
    {
        var processedGroup = FlowStringProcessor.Process(data.GroupName, flowVars);

        if (!FlowStringProcessor.TryProcessInt(data.TimeoutAmount, flowVars, out var processedTimeout))
        {
            Logger.LogWarning($"Unable to process {data.TimeoutAmount} into a number, verify it is a number or variables exist");
            return ExecutionResult.Failed($"Unable to process integer value from {data.TimeoutAmount}");
        }
        
        var timeoutTimespan = data.TimeoutUnitsType.ToTimeSpan(processedTimeout);
        Logger.Information($"Attempting to get exclusive lock for {processedGroup} with {timeoutTimespan.TotalSeconds}s timeout");
        using var cancellationTokenTimeout = new CancellationTokenSource((int)timeoutTimespan.TotalMilliseconds);
        var succeeded = await ExclusiveExecutionHandler.RequestLockFor(processedGroup, cancellationTokenTimeout.Token);

        if (!succeeded) 
        { return ExecutionResult.Failed("Unable to get exclusive lock on group"); }
        
        return ExecutionResult.Success(ExclusiveExecutionTaskData.SubTaskKey);
    }

    public Task OnSubTasksFinished(IFlowTaskData data, IVariables flowVars, ExecutionResultType executionResultType)
    {
        var typedData = data as ExclusiveExecutionTaskData;
        var processedGroup = FlowStringProcessor.Process(typedData.GroupName, flowVars);

        Logger.Information($"Freeing exclusive lock for {processedGroup}");
        ExclusiveExecutionHandler.FreeLockFor(processedGroup);
        return Task.CompletedTask;
    }
}