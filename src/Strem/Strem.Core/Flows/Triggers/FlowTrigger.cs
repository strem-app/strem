using System.Reactive;
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
    
    public ILogger<IFlowTrigger> Logger { get; }
    public IFlowStringProcessor FlowStringProcessor { get; }
    public IAppState AppState { get; }

    protected FlowTrigger(ILogger<IFlowTrigger> logger, IFlowStringProcessor flowStringProcessor, IAppState appState)
    {
        Logger = logger;
        FlowStringProcessor = flowStringProcessor;
        AppState = appState;
    }

    public abstract IObservable<IVariables> Execute(T data);

    public IObservable<IVariables> Execute(object data)
    {
        if(data is T typedData)
        { return Execute(typedData); }

        throw new ArgumentException($"Task type is {data.GetType()} not {typeof(T)}");
    }
}