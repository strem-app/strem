using Strem.Core.Flows.Tasks;
using Strem.OBS.Types;

namespace Strem.OBS.Flows.Tasks;

public class SetSourceVisibilityTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "set-source-visibility";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string SceneName { get; set; }
    public string SourceName { get; set; }
    public VisibilityStatus Visibility { get; set; }
}