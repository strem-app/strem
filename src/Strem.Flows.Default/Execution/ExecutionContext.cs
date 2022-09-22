using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Variables;
using Strem.Flows.Executors;

namespace Strem.Flows.Default.Execution;

public class ExecutionContext
{
    public ILogger Logger { get; }
    public IVariables UserVars { get; }
    public IVariables TempVars { get; }
    public IVariables FlowVars { get; }
    public IEventBus EventBus { get; }
    public IFlowExecutor FlowExecutor { get; }

    public ExecutionContext(ILogger logger, IVariables userVars, IVariables tempVars, IVariables flowVars, IEventBus eventBus, IFlowExecutor flowExecutor)
    {
        Logger = logger;
        UserVars = userVars;
        TempVars = tempVars;
        FlowVars = flowVars;
        EventBus = eventBus;
        FlowExecutor = flowExecutor;
    }
}