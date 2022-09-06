using System.ComponentModel.DataAnnotations;
using System.Reactive.Disposables;
using Strem.Core.Events.Bus;
using Strem.Core.Events.Flows;
using Strem.Core.Events.Flows.Tasks;
using Strem.Core.Events.Flows.Triggers;
using Strem.Core.Extensions;
using Strem.Core.Flows.Registries.Tasks;
using Strem.Core.Flows.Registries.Triggers;
using Strem.Core.Flows.Tasks;
using Strem.Core.Flows.Triggers;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Validation;
using Strem.Core.Variables;

namespace Strem.Core.Flows.Executors;

public class FlowExecutionEngine : IFlowExecutionEngine
{
    public IFlowStore FlowStore { get; }
    public IAppState AppState { get; }
    public IEventBus EventBus { get; }
    public ITaskRegistry TaskRegistry { get; }
    public ITriggerRegistry TriggerRegistry { get; }
    public ILogger<FlowExecutionEngine> Logger { get; }
    public IDataValidator DataValidator { get; }

    public CompositeDisposable InternalSubs { get; } = new();
    public Dictionary<Guid, CompositeDisposable> FlowSubscriptions { get; } = new();
    public List<FlowExecutionLog> Logs { get; } = new();
    public IReadOnlyCollection<FlowExecutionLog> ExecutionLogs => Logs;

    public FlowExecutionEngine(IFlowStore flowStore, IEventBus eventBus, ITaskRegistry taskRegistry, ITriggerRegistry triggerRegistry, ILogger<FlowExecutionEngine> logger, IAppState appState, IDataValidator dataValidator)
    {
        FlowStore = flowStore;
        EventBus = eventBus;
        TaskRegistry = taskRegistry;
        TriggerRegistry = triggerRegistry;
        Logger = logger;
        AppState = appState;
        DataValidator = dataValidator;
    }

    public async Task StartEngine()
    {
        EventBus.Receive<FlowAddedEvent>()
            .Subscribe(x => SetupFlow(FlowStore.Get(x.FlowId)))
            .AddTo(InternalSubs);

        EventBus.Receive<FlowRemovedEvent>()
            .Subscribe(x => RemoveFlow(x.FlowId))
            .AddTo(InternalSubs);
        
        // TODO: Have this just reset a single trigger not all flow triggers
        EventBus.Receive<FlowTriggerChangedEvent>()
            .Subscribe(x =>
            {
                RemoveFlow(x.FlowId);
                SetupFlow(FlowStore.Get(x.FlowId));
            })
            .AddTo(InternalSubs);
        
        foreach(var flow in FlowStore.Flows)
        { await SetupFlow(flow); }
    }
    
    public async Task SetupFlow(Flow flow)
    {
        if (flow.TriggerData.Count == 0) { return; }
        
        var flowSubs = new CompositeDisposable();
        FlowSubscriptions.Add(flow.Id, flowSubs);
        
        foreach (var triggerData in flow.TriggerData)
        { await SetupTrigger(flow, triggerData, flowSubs); }
    }

    public async Task SetupTrigger(Flow flow, IFlowTriggerData triggerData, CompositeDisposable flowSubs)
    {
        var trigger = TriggerRegistry.Get(triggerData.Code)?.Trigger;
        if (trigger == null)
        {
            Logger.LogWarning($"Trigger cant be found for trigger code: {triggerData.Code} (v{triggerData.Version})");
            return;
        }

        var validationResults = DataValidator.Validate(triggerData);
        if (!validationResults.IsValid)
        {
            Logger.LogWarning($"Trigger data contains invalid data for {triggerData.Id}|{triggerData.Code}, with errors {string.Join(" | ", validationResults.Errors)}");
            return;
        }
            
        var observable = await trigger.Execute(triggerData);
        observable.Subscribe(vars =>
            {
                EventBus.PublishAsync(new FlowTriggerStarted(flow.Id, triggerData.Id));
                ExecuteFlow(flow, vars);
                EventBus.PublishAsync(new FlowTriggerFinished(flow.Id, triggerData.Id));
            })
            .AddTo(flowSubs);
    }

    public void RemoveFlow(Guid flowId)
    {
        if (!FlowSubscriptions.ContainsKey(flowId)) { return; }
        
        FlowSubscriptions[flowId].Dispose();
        FlowSubscriptions.Remove(flowId);
    }
    
    public IVariables CollateActiveVariables(IVariables flowVariables)
    {
        var allVariables = flowVariables.GetAll()
            .Concat(AppState.TransientVariables.GetAll())
            .Concat(AppState.UserVariables.GetAll());

        return new Variables.Variables(allVariables.ToDictionary(x => x.Key, x => x.Value));
    }

    public void CancelExecution(Flow flow, IFlowTaskData currentTaskData, ExecutionResultType executionResultType, IVariables flowVariables, FlowExecutionLog executionLog)
    {
        EventBus.PublishAsync(new FlowFinishedEvent(flow.Id));
                
        executionLog.EndTime = DateTime.Now;
        executionLog.EndVariables = CollateActiveVariables(flowVariables);
        executionLog.ExecutionResultType = executionResultType;
        executionLog.ElementExecutionSummary.Add($"{currentTaskData.Code} - {executionResultType}");
    }

    public FlowExecutionLog SetupLogFor(Flow flow, IVariables flowVariables)
    {
        var executionLog = new FlowExecutionLog
        {
            FlowId = flow.Id,
            FlowName = flow.Name,
            StartTime = DateTime.Now,
            StartVariables = CollateActiveVariables(flowVariables)
        };
        Logs.Add(executionLog);
        return executionLog;
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

    public async Task ExecuteFlow(Flow flow, IVariables? flowVariables = null)
    {
        if(flow.TaskData.Count == 0) { return; }
        if(!flow.Enabled) { return; }
        
        flowVariables ??= new Variables.Variables();
        var executionLog = SetupLogFor(flow, flowVariables);
        EventBus.PublishAsync(new FlowStartedEvent(flow.Id));
        
        foreach (var taskData in flow.TaskData)
        {
            var executionResult = await ExecuteTask(flow, taskData, flowVariables, executionLog);
            if (executionResult.ResultType is ExecutionResultType.Failed or ExecutionResultType.CascadingFailure)
            {
                CancelExecution(flow, taskData, executionResult.ResultType, flowVariables, executionLog);
                return;
            }
        }
        
        EventBus.PublishAsync(new FlowFinishedEvent(flow.Id));
        executionLog.EndTime = DateTime.Now;
        executionLog.EndVariables = CollateActiveVariables(flowVariables);
        executionLog.ExecutionResultType = ExecutionResultType.Success;
    }

    public async Task ExecuteFlow(Guid flowId, IVariables flowVariables = null)
    {
        if(flowId == Guid.Empty) { return; }
        var matchingFlow = FlowStore.Flows.SingleOrDefault(x => x.Id == flowId);
        await ExecuteFlow(matchingFlow, flowVariables);
    }

    public void Dispose()
    {
        InternalSubs.Dispose();
        FlowSubscriptions.DisposeAll();
    }
}