﻿using System.Reactive.Disposables;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Events;
using Strem.Flows.Events.Triggers;
using Strem.Flows.Executors.Logging;
using Strem.Flows.Services.Stores;
using Strem.Flows.Types;

namespace Strem.Flows.Executors;

public class FlowExecutionEngine : IFlowExecutionEngine
{
    public IFlowStore FlowStore { get; }
    public IAppState AppState { get; }
    public IEventBus EventBus { get; }
    public ILogger<FlowExecutionEngine> Logger { get; }
    
    public ITaskExecutor TaskExecutor { get; }
    public ITriggerExecutor TriggerExecutor { get; }
    public IFlowExecutionLogger FlowExecutionLogger { get; }

    public CompositeDisposable InternalSubs { get; } = new();
    public Dictionary<Guid, CompositeDisposable> FlowSubscriptions { get; } = new();

    public FlowExecutionEngine(IFlowStore flowStore, IAppState appState, IEventBus eventBus, ILogger<FlowExecutionEngine> logger, ITaskExecutor taskExecutor, ITriggerExecutor triggerExecutor, IFlowExecutionLogger flowExecutionLogger)
    {
        FlowStore = flowStore;
        AppState = appState;
        EventBus = eventBus;
        Logger = logger;
        TaskExecutor = taskExecutor;
        TriggerExecutor = triggerExecutor;
        FlowExecutionLogger = flowExecutionLogger;
    }

    public async Task StartEngine()
    {
        EventBus.Receive<FlowAddedEvent>()
            .Subscribe(x => ResetFlow(x.FlowId))
            .AddTo(InternalSubs);

        EventBus.Receive<FlowRemovedEvent>()
            .Subscribe(x => ResetFlow(x.FlowId))
            .AddTo(InternalSubs);
        
        // TODO: Have this just reset a single trigger not all flow triggers
        EventBus.Receive<FlowTriggerChangedEvent>()
            .Subscribe(async x => ResetFlow(x.FlowId))
            .AddTo(InternalSubs);

        foreach (var flow in FlowStore.Data)
        {
            try
            { await SetupFlow(flow); }
            catch (Exception e)
            { Logger.Error($"Unable to setup flow [{flow.Name}|{flow.Id}], error thrown: {e.Message}"); }
        }
    }
    
    public async Task SetupFlow(Flow flow)
    {
        if (flow.TriggerData.Count == 0) { return; }
        
        var flowSubs = new CompositeDisposable();
        FlowSubscriptions.Add(flow.Id, flowSubs);
        
        foreach (var triggerData in flow.TriggerData)
        { 
            await TriggerExecutor.SetupTrigger(flow, triggerData, flowSubs, vars => {
                EventBus.PublishAsync(new FlowTriggerStartedEvent(flow.Id, triggerData.Id));
                ExecuteFlow(flow, vars);
                EventBus.PublishAsync(new FlowTriggerFinishedEvent(flow.Id, triggerData.Id));
            }); 
        }
    }

    public async Task ResetFlow(Guid flowId)
    {
        RemoveFlow(flowId);
        await SetupFlow(FlowStore.Get(flowId));
    }

    public void RemoveFlow(Guid flowId)
    {
        if (!FlowSubscriptions.ContainsKey(flowId)) { return; }
        
        FlowSubscriptions[flowId].Dispose();
        FlowSubscriptions.Remove(flowId);
    }
    
    public void FinishFlow(Flow flow, ExecutionResultType executionResultType, IVariables flowVariables, FlowExecutionLog executionLog, IFlowTaskData? currentTaskData = null)
    {
        EventBus.PublishAsync(new FlowFinishedEvent(flow.Id, executionResultType));
        FlowExecutionLogger.CloseLogFor(executionLog, flowVariables, executionResultType, currentTaskData);
    }

    public async Task ExecuteFlow(Flow flow, IVariables? flowVariables = null)
    {
        if(flow.TaskData.Count == 0) { return; }
        if(!flow.Enabled) { return; }
        
        flowVariables ??= new Core.Variables.Variables();
        var executionLog = FlowExecutionLogger.CreateLogFor(flow, flowVariables);
        EventBus.PublishAsync(new FlowStartedEvent(flow.Id));
        
        foreach (var taskData in flow.TaskData)
        {
            var executionResult = await TaskExecutor.ExecuteTask(flow, taskData, flowVariables, executionLog);
            if (executionResult.ResultType is ExecutionResultType.Failed or ExecutionResultType.CascadingFailure)
            {
                FinishFlow(flow, executionResult.ResultType, flowVariables, executionLog, taskData);
                return;
            }
        }
        FinishFlow(flow, ExecutionResultType.Success, flowVariables, executionLog);
    }

    public async Task ExecuteFlow(Guid flowId, IVariables flowVariables = null)
    {
        if(flowId == Guid.Empty) { return; }
        var matchingFlow = FlowStore.Get(flowId);
        if (matchingFlow == null)
        {
            Logger.Error($"No flow found for {flowId}");
            return;
        }
        await ExecuteFlow(matchingFlow, flowVariables);
    }

    public void Dispose()
    {
        InternalSubs.Dispose();
        FlowSubscriptions.DisposeAll();
    }
}