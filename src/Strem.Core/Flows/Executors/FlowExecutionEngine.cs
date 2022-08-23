using System.Reactive.Disposables;
using Strem.Core.Events.Bus;
using Strem.Core.Events.Flows;
using Strem.Core.Events.Flows.Tasks;
using Strem.Core.Events.Flows.Triggers;
using Strem.Core.Extensions;
using Strem.Core.Flows.Registries;
using Strem.Core.Flows.Registries.Tasks;
using Strem.Core.Flows.Registries.Triggers;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
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

    public CompositeDisposable InternalSubs { get; } = new();
    public Dictionary<Guid, CompositeDisposable> FlowSubscriptions { get; } = new();
    public List<FlowExecutionLog> Logs { get; } = new();
    public IReadOnlyCollection<FlowExecutionLog> ExecutionLogs => Logs;

    public FlowExecutionEngine(IFlowStore flowStore, IEventBus eventBus, ITaskRegistry taskRegistry, ITriggerRegistry triggerRegistry, ILogger<FlowExecutionEngine> logger, IAppState appState)
    {
        FlowStore = flowStore;
        EventBus = eventBus;
        TaskRegistry = taskRegistry;
        TriggerRegistry = triggerRegistry;
        Logger = logger;
        AppState = appState;
    }

    public void StartEngine()
    {
        EventBus.Receive<FlowAddedEvent>()
            .Subscribe(x => SetupFlow(FlowStore.Get(x.FlowId)))
            .AddTo(InternalSubs);

        EventBus.Receive<FlowRemovedEvent>()
            .Subscribe(x => RemoveFlow(x.FlowId))
            .AddTo(InternalSubs);
        
        EventBus.Receive<FlowTriggersChangedEvent>()
            .Subscribe(x =>
            {
                RemoveFlow(x.FlowId);
                SetupFlow(FlowStore.Get(x.FlowId));
            })
            .AddTo(InternalSubs);
        
        foreach(var flow in FlowStore.Flows)
        { SetupFlow(flow); }
    }
    
    public void SetupFlow(Flow flow)
    {
        if (flow.TriggerData.Count == 0) { return; }
        
        var flowSubs = new CompositeDisposable();
        FlowSubscriptions.Add(flow.Id, flowSubs);
        
        foreach (var triggerData in flow.TriggerData)
        {
            var trigger = TriggerRegistry.Get(triggerData.Code)?.Trigger;
            if (trigger == null)
            {
                Logger.LogWarning($"Trigger cant be found for trigger code: {triggerData.Code} (v{triggerData.Version})");
                continue;
            }
            
            trigger.Execute(triggerData)
                .Subscribe(vars =>
                {
                    EventBus.PublishAsync(new FlowTriggerStarted(flow.Id, triggerData.Id));
                    ExecuteFlow(flow, vars);
                    EventBus.PublishAsync(new FlowTriggerFinished(flow.Id, triggerData.Id));
                })
                .AddTo(flowSubs);
        }
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

    public void CancelExecution(Flow flow, IFlowTaskData currentTaskData, ExecutionResult executionResult, IVariables flowVariables, FlowExecutionLog executionLog)
    {
        EventBus.PublishAsync(new FlowTaskFinished(flow.Id, currentTaskData.Id));
        EventBus.PublishAsync(new FlowFinishedEvent(flow.Id));
                
        executionLog.EndTime = DateTime.Now;
        executionLog.EndVariables = CollateActiveVariables(flowVariables);
        executionLog.ExecutionResult = executionResult;
        executionLog.ElementExecutionSummary.Add($"{currentTaskData.Code} - {executionResult}");
    }

    public async Task ExecuteFlow(Flow flow, IVariables flowVariables = null)
    {
        if(flow.TaskData.Count == 0) { return; }
        if(!flow.Enabled) { return; }
        
        // TODO: Maybe need to handle multiple calls here, should it run in parallel or queue or forget
        //if(FlowExecutions.ContainsKey(flow.Id)) { return; }
        
        if (flowVariables == null)
        { flowVariables = new Variables.Variables(); }

        var executionLog = new FlowExecutionLog
        {
            FlowId = flow.Id,
            FlowName = flow.Name,
            StartTime = DateTime.Now,
            StartVariables = CollateActiveVariables(flowVariables)
        };
        Logs.Add(executionLog);

        EventBus.PublishAsync(new FlowStartedEvent(flow.Id));
        
        foreach (var taskData in flow.TaskData)
        {
            var task = TaskRegistry.Get(taskData.Code)?.Task;

            if (task == null)
            {
                Logger.LogWarning($"Task cant be found for task code: {taskData.Code} (v{taskData.Version})");
                continue;
            }
            
            EventBus.PublishAsync(new FlowTaskStarted(flow.Id, taskData.Id));
            try
            {
                var executionResult = await task.Execute(taskData, flowVariables);
                if (executionResult == ExecutionResult.Failed)
                {
                    Logger.Information($"Failed Executing Flow {flow.Name} | Task Info {taskData.Code}[{taskData.Id}]");
                    CancelExecution(flow, taskData, executionResult, flowVariables, executionLog);
                    return;
                }
                
                EventBus.PublishAsync(new FlowTaskFinished(flow.Id, taskData.Id));
                executionLog.ElementExecutionSummary.Add($"{taskData.Code} - {executionResult}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error Executing Flow {flow.Name} | Task Info {taskData.Code}[{taskData.Id}] | Error: {ex.Message}");
                CancelExecution(flow, taskData, ExecutionResult.Failed, flowVariables, executionLog);
                return;
            }
        }
        
        EventBus.PublishAsync(new FlowFinishedEvent(flow.Id));
        executionLog.EndTime = DateTime.Now;
        executionLog.EndVariables = CollateActiveVariables(flowVariables);
        executionLog.ExecutionResult = ExecutionResult.Success;
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