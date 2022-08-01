using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.Variables;
using Strem.Infrastructure.Flows.Tasks.Data;

namespace Strem.Infrastructure.Flows.Tasks;

public class WriteToLogTask : IFlowTask<WriteToLogTaskData>
{
    public static readonly string Code = "write-to-log";
    public static readonly string Version = "1.0.0";
    public string TaskCode => Code;
    public string TaskVersion => Version;
    
    public string Name => "Write To Log";
    public string Description => "Writes the text out to the log file, useful for debugging";

    public ILogger<WriteToLogTask> Logger { get; }
    public FlowStringProcessor FlowStringProcessor { get; }

    public WriteToLogTask(ILogger<WriteToLogTask> logger, FlowStringProcessor flowStringProcessor)
    {
        Logger = logger;
        FlowStringProcessor = flowStringProcessor;
    }

    public async Task Execute(WriteToLogTaskData data, IVariables flowVars)
    {
        var processedText = FlowStringProcessor.Process(data.Text, flowVars);
        Logger.Information(processedText);
    }
}