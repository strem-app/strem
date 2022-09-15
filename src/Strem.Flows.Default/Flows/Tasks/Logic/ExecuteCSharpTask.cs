using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Default.Extensions;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Westwind.Scripting;
using ExecutionContext = Strem.Flows.Default.Execution.ExecutionContext;

namespace Strem.Flows.Default.Flows.Tasks.Logic;

public class ExecuteCSharpTask : FlowTask<ExecuteCSharpTaskData>
{
    public override string Code => ExecuteCSharpTaskData.TaskCode;
    public override string Version => ExecuteCSharpTaskData.TaskVersion;
    
    public override string Name => "Execute C# Script";
    public override string Category => "Logic";
    public override string Description => "Allows you to execute your own C# logic";
    
    public ExecuteCSharpTask(ILogger<FlowTask<ExecuteCSharpTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {}

    public override bool CanExecute() => true;

    public override async Task<ExecutionResult> Execute(ExecuteCSharpTaskData data, IVariables flowVars)
    {
        var scriptExecutor = new CSharpScriptExecution { AllowReferencesInCode = true, ThrowExceptions = true };
        var context = new ExecutionContext(Logger, AppState.UserVariables, AppState.TransientVariables, flowVars, EventBus);
        var executor = scriptExecutor.CompileMethodExecutor(data.CSharpCode);
        await executor.Execute(context);
        
        return ExecutionResult.Success();
    }
}