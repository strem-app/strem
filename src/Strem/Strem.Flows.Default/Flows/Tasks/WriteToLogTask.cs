using Microsoft.Extensions.Logging;
using Strem.Core.Extensions;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
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

    public ILogger<WriteToLogTask> Logger { get; }
    public IFlowStringProcessor FlowStringProcessor { get; }

    public WriteToLogTask(ILogger<WriteToLogTask> logger, IFlowStringProcessor flowStringProcessor)
    {
        Logger = logger;
        FlowStringProcessor = flowStringProcessor;
    }

    public override async Task Execute(WriteToLogTaskData data, IVariables flowVars)
    {
        var processedText = FlowStringProcessor.Process(data.Text, flowVars);
        Logger.Information(processedText);
    }
}