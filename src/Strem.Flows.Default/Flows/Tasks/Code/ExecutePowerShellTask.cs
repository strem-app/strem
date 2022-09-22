using System.Management.Automation;
using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Data.Extensions;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Executors;
using Strem.Flows.Processors;

namespace Strem.Flows.Default.Flows.Tasks.Code;

public class ExecutePowerShellTask : FlowTask<ExecutePowerShellTaskData>
{
    public override string Code => ExecutePowerShellTaskData.TaskCode;
    public override string Version => ExecutePowerShellTaskData.TaskVersion;
    
    public override string Name => "Execute PowerShell Script";
    public override string Category => "Code";
    public override string Description => "Allows you to execute your own powershell scripts";
    
    public IFlowExecutor FlowExecutor { get; }
    
    public ExecutePowerShellTask(ILogger<FlowTask<ExecutePowerShellTaskData>> logger, IFlowStringProcessor flowStringProcessor,
        IAppState appState, IEventBus eventBus, IFlowExecutor flowExecutor) : base(logger, flowStringProcessor,
        appState, eventBus)
    {
        FlowExecutor = flowExecutor;
    }

    public override bool CanExecute() => true;

    public IReadOnlyCollection<KeyValuePair<string, string>> GetVariables(IVariables flowVars)
    {
        return flowVars.Combine(AppState.UserVariables, AppState.TransientVariables)
            .Select(x => new KeyValuePair<string, string>(x.Key.ToCompositeKey(), x.Value))
            .ToArray();
    }

    public override async Task<ExecutionResult> Execute(ExecutePowerShellTaskData data, IVariables flowVars)
    {
        using var ps = PowerShell.Create();

        GetVariables(flowVars).ForEach(x => ps.Runspace.SessionStateProxy.SetVariable(x.Key, x.Value));
        await ps.AddScript(data.PSCode).InvokeAsync();
        if (!ps.HadErrors)
        { return ExecutionResult.Success(); }
        
        var exception = ps.Streams.Error.ElementAt(0)?.Exception?.Message ?? "Unknown Error";
        Logger.Error($"Powershell script run failed with: {exception}");
        return ExecutionResult.Failed(exception);
    }
}