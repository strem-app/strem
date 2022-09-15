using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Variables;

namespace Strem.Flows.Default.Execution;

public class ExecutionContext
{
    public ILogger Logger { get; }
    public IVariables UserVars { get; }
    public IVariables TempVars { get; }
    public IVariables FlowVars { get; }
    public IEventBus EventBus { get; }

    public ExecutionContext(ILogger logger, IVariables userVars, IVariables tempVars, IVariables flowVars, IEventBus eventBus)
    {
        Logger = logger;
        UserVars = userVars;
        TempVars = tempVars;
        FlowVars = flowVars;
        EventBus = eventBus;
    }
}