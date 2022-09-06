using Strem.Core.Events.Bus;
using Strem.Core.Events.Flows.Tasks;
using Strem.Core.Extensions;
using Strem.Core.Flows.Executors.Logging;
using Strem.Core.Flows.Registries.Tasks;
using Strem.Core.Flows.Tasks;
using Strem.Core.Types;
using Strem.Core.Validation;
using Strem.Core.Variables;

namespace Strem.Core.Flows.Executors;

public class TaskExecutor : ITaskExecutor
{
    public ITaskRegistry TaskRegistry { get; }
    public ILogger<TaskExecutor> Logger { get; }
    public IDataValidator DataValidator { get; }
    public IEventBus EventBus { get; }

    public TaskExecutor(ITaskRegistry taskRegistry, ILogger<TaskExecutor> logger, IDataValidator dataValidator, IEventBus eventBus)
    {
        TaskRegistry = taskRegistry;
        Logger = logger;
        DataValidator = dataValidator;
        EventBus = eventBus;
    }

    public async Task<ExecutionResult> ExecuteTask(Flow flow, IFlowTaskData taskData, IVariables flowVariables, FlowExecutionLog executionLog)
    {
        var task = TaskRegistry.Get(taskData.Code)?.Task;

        if (task == null)
        {
            Logger.LogWarning($"Task cant be found for task code: {taskData.Code} (v{taskData.Version})");
            return ExecutionResult.FailedButContinue("Task Failed, See Log");
        }
        
        var validationResults = DataValidator.Validate(taskData);
        if (!validationResults.IsValid)
        {
            Logger.LogWarning($"Task data contains invalid data for {taskData.Id}|{taskData.Code}, with errors {string.Join(" | ", validationResults.Errors)}");
            return ExecutionResult.Failed("Task Data Failed Validation, See Log");
        }
            
        EventBus.PublishAsync(new FlowTaskStarted(flow.Id, taskData.Id));
        try
        {
            var executionResult = await task.Execute(taskData, flowVariables);
            
            if (executionResult.ResultType is ExecutionResultType.Failed or ExecutionResultType.CascadingFailure)
            {
                Logger.Warning($"Failed Executing Flow {flow.Name} | Task Info {taskData.Code}[{taskData.Id}]");
                EventBus.PublishAsync(new FlowTaskFinished(flow.Id, taskData.Id));
                return executionResult;
            }

            if (taskData is IHasSubTaskData subTaskData && executionResult.SubTaskKeys.Length > 0)
            {
                foreach (var subTaskKey in executionResult.SubTaskKeys)
                {
                    if(!subTaskData.SubTasks.ContainsKey(subTaskKey)) { continue; }
                    executionLog.ElementExecutionSummary.Add($"Entering Sub Task For {taskData.Code} With Key {subTaskKey}");
                    foreach (var subTaskDataEntry in subTaskData.SubTasks[subTaskKey])
                    {
                        var subExecutionResult = await ExecuteTask(flow, subTaskDataEntry, flowVariables, executionLog);
                        if (subExecutionResult.ResultType == ExecutionResultType.Failed) { break; }
                        if (subExecutionResult.ResultType == ExecutionResultType.CascadingFailure) { return executionResult; }
                    }
                    executionLog.ElementExecutionSummary.Add($"Finished Sub Task For {taskData.Code} With Key {subTaskKey}");
                }
            }

            EventBus.PublishAsync(new FlowTaskFinished(flow.Id, taskData.Id));
            executionLog.ElementExecutionSummary.Add($"{taskData.Code} - {executionResult}");
            return executionResult;
        }
        catch (Exception ex)
        {
            Logger.Error($"Error Executing Flow {flow.Name} | Task Info {taskData.Code}[{taskData.Id}] | Error: {ex.Message}");
            EventBus.PublishAsync(new FlowTaskFinished(flow.Id, taskData.Id));
            return ExecutionResult.Failed($"Task Failed With Exception: {ex.Message}");
        }
    }
}