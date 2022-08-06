using Strem.Core.Events.Bus;
using Strem.Core.Flows.Processors;
using Strem.Core.State;
using Strem.Core.Variables;

namespace Strem.Core.Flows.Triggers;

public abstract class FlowTrigger<T> : IFlowTrigger 
    where T : IFlowTriggerData
{
    public abstract string Version { get; }
    public abstract string Code { get; }
    public abstract string Name { get; }
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

    public abstract IObservable<IVariables> Execute(T data);

    public abstract bool CanExecute();

    public IObservable<IVariables> Execute(object data)
    {
        if(data is T typedData)
        { return Execute(typedData); }

        throw new ArgumentException($"Task type is {data.GetType()} not {typeof(T)}");
    }
}