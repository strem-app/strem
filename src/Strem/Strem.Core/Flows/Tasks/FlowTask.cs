using Strem.Core.Events.Bus;
using Strem.Core.Flows.Processors;
using Strem.Core.State;
using Strem.Core.Variables;

namespace Strem.Core.Flows.Tasks;

public abstract class FlowTask<T> : IFlowTask
    where T : IFlowTaskData
{
    public abstract string Version { get; }
    public abstract string Code { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    
    public IEventBus EventBus { get; }
    public ILogger<IFlowTask> Logger { get; }
    public IFlowStringProcessor FlowStringProcessor { get; }
    public IAppState AppState { get; }

    protected FlowTask(ILogger<IFlowTask> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus)
    {
        Logger = logger;
        FlowStringProcessor = flowStringProcessor;
        AppState = appState;
        EventBus = eventBus;
    }

    public abstract bool CanExecute();

    public Task Execute(object data, IVariables flowVars)
    {
        if(data is T typedData)
        { return Execute(typedData, flowVars); }

        throw new ArgumentException($"Task type is {data.GetType()} not {typeof(T)}");
    }

    public abstract Task Execute(T data, IVariables flowVars);
}