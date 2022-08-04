using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Default.Flows.Tasks.Data;

namespace Strem.Flows.Default.Flows.Tasks;

public class WriteToLogTask : FlowTask<WriteToLogTaskData>
{
    public static readonly string TaskCode = "write-to-log";
    public static readonly string TaskVersion = "1.0.0";
    public override string Code => TaskCode;
    public override string Version => TaskVersion;
    
    public override string Name => "Write To Log";
    public override string Description => "Writes the text out to the log file, useful for debugging";

    public WriteToLogTask(ILogger<IFlowTask> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    public override async Task Execute(WriteToLogTaskData data, IVariables flowVars)
    {
        var processedText = FlowStringProcessor.Process(data.Text, flowVars);
        Logger.Information(processedText);
    }
}