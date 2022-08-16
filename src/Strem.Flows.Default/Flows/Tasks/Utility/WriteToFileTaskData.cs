using Strem.Core.Flows.Tasks;
using Strem.Core.Types;

namespace Strem.Flows.Default.Flows.Tasks.Utility;

public class WriteToFileTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "write-to-file";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string Text { get; set; }
    public string FilePath { get; set; }
    public FileInteractionType InteractionType { get; set; }
}