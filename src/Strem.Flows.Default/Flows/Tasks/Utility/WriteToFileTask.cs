using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;

namespace Strem.Flows.Default.Flows.Tasks.Utility;

public class WriteToFileTask : FlowTask<WriteToFileTaskData>
{
    public override string Code => WriteToFileTaskData.TaskCode;
    public override string Version => WriteToFileTaskData.TaskVersion;
    
    public override string Name => "Write To File";
    public override string Category => "Utility";
    public override string Description => "Writes the content out to a file";

    public WriteToFileTask(ILogger<FlowTask<WriteToFileTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    public override bool CanExecute() => true;

    public void CreateFileIfNeeded(string filePath)
    {
        if(File.Exists(filePath)) { return; }

        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        { Directory.CreateDirectory(directory); }
        
        File.Create(filePath).Dispose();
    }

    public async Task WriteToFile(WriteToFileTaskData data, IVariables flowVars)
    {
        using var file = File.Open(data.FilePath, FileMode.Open, FileAccess.ReadWrite);
        if (data.InteractionType == FileInteractionType.Replace)
        {
            file.SetLength(0);
            file.Close();
        }

        var processedContent = FlowStringProcessor.Process(data.Text, flowVars);
        if (data.InteractionType == FileInteractionType.Prepend)
        {
            var currentContent = await File.ReadAllLinesAsync(data.FilePath);
            var newContent = new[] { processedContent }.Concat(currentContent);
            await File.WriteAllLinesAsync(data.FilePath, newContent);
            return;
        }

        await File.AppendAllLinesAsync(data.FilePath, new []{processedContent});
    }
    
    public override async Task<ExecutionResult> Execute(WriteToFileTaskData data, IVariables flowVars)
    {
        if (string.IsNullOrEmpty(data.FilePath))
        {
            Logger.Warning("No path provided for Write To Path task");
            return ExecutionResult.Failed("No file path provided");
        }
        
        CreateFileIfNeeded(data.FilePath);
        try
        {
            await WriteToFile(data, flowVars);
            return ExecutionResult.Success();
        }
        catch (Exception ex)
        {
            Logger.Error($"Error writing to File [{data.FilePath}]: {ex.Message}");
            return ExecutionResult.Failed($"Cannot write to file: [{data.FilePath}] | {ex.Message}");
        }
    }
}