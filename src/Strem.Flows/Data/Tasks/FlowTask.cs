using Strem.Core.Events.Bus;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Executors;
using Strem.Flows.Processors;

namespace Strem.Flows.Data.Tasks;

public abstract class FlowTask<T> : IFlowTask
    where T : IFlowTaskData
{
    public abstract string Version { get; }
    public abstract string Code { get; }
    public abstract string Name { get; }
    public abstract string Category { get; }
    public abstract string Description { get; }

    public virtual VariableDescriptor[] VariableOutputs { get; } = Array.Empty<VariableDescriptor>();
    
    public IEventBus EventBus { get; }
    public ILogger<FlowTask<T>> Logger { get; }
    public IFlowStringProcessor FlowStringProcessor { get; }
    public IAppState AppState { get; }

    protected FlowTask(ILogger<FlowTask<T>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus)
    {
        Logger = logger;
        FlowStringProcessor = flowStringProcessor;
        AppState = appState;
        EventBus = eventBus;
    }

    public abstract bool CanExecute();

    public Task<ExecutionResult> Execute(object data, IVariables flowVars)
    {
        if(data is T typedData)
        { return Execute(typedData, flowVars); }

        throw new ArgumentException($"Task type is {data.GetType()} not {typeof(T)}");
    }

    public abstract Task<ExecutionResult> Execute(T data, IVariables flowVars);
}