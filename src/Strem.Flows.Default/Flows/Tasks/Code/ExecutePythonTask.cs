using IronPython.Hosting;
using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using ExecutionContext = Strem.Flows.Default.Execution.ExecutionContext;

namespace Strem.Flows.Default.Flows.Tasks.Code;

public class ExecutePythonTask : FlowTask<ExecutePythonTaskData>
{
    public override string Code => ExecutePythonTaskData.TaskCode;
    public override string Version => ExecutePythonTaskData.TaskVersion;
    
    public override string Name => "Execute Python Script";
    public override string Category => "Code";
    public override string Description => "Allows you to execute your own python logic (via IronPython)";
    
    public ExecutePythonTask(ILogger<FlowTask<ExecutePythonTaskData>> logger, IFlowStringProcessor flowStringProcessor,
        IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor,
        appState, eventBus)
    {}

    public override bool CanExecute() => true;

    public override async Task<ExecutionResult> Execute(ExecutePythonTaskData data, IVariables flowVars)
    {
        var engine = Python.CreateEngine();
        var scope = engine.CreateScope();
        engine.Execute(data.PythonCode, scope);
        var executor = scope.GetVariable("execute");
        
        var context = new ExecutionContext(Logger, AppState.UserVariables, AppState.TransientVariables, flowVars, EventBus);
        executor(context);
        
        return ExecutionResult.Success();
    }
}