﻿using Strem.Core.Events.Bus;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Processors;

namespace Strem.Flows.Data.Triggers;

public abstract class FlowTrigger<T> : IFlowTrigger 
    where T : IFlowTriggerData
{
    public abstract string Version { get; }
    public abstract string Code { get; }
    public abstract string Name { get; }
    public abstract string Category { get; }
    public abstract string Description { get; }

    public virtual VariableDescriptor[] VariableOutputs { get; } = Array.Empty<VariableDescriptor>();

    public ILogger<FlowTrigger<T>> Logger { get; }
    public IFlowStringProcessor FlowStringProcessor { get; }
    public IAppState AppState { get; }
    public IEventBus EventBus { get; }

    protected FlowTrigger(ILogger<FlowTrigger<T>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus)
    {
        Logger = logger;
        FlowStringProcessor = flowStringProcessor;
        AppState = appState;
        EventBus = eventBus;
    }

    public abstract Task<IObservable<IVariables>> Execute(T data);

    public abstract bool CanExecute();

    public Task<IObservable<IVariables>> Execute(object data)
    {
        if(data is T typedData)
        { return Execute(typedData); }

        throw new ArgumentException($"Task type is {data.GetType()} not {typeof(T)}");
    }
}