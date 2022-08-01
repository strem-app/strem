using Strem.Core.Flows;
using Strem.Core.Flows.Tasks;

namespace Strem.Infrastructure.Flows.Tasks.Data;

public class WriteToLogTaskData : IFlowTaskData
{
    public string TaskCode => WriteToLogTask.Code;
    public string Version { get; set; } = WriteToLogTask.Version;
    
    public string Text { get; set; }
}