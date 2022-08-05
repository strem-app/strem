using Strem.Core.Flows.Tasks;
using Strem.Core.Types;
using Strem.Core.Utils;

namespace Strem.Flows.Default.Flows.Tasks.Data;

public class WaitForPeriodTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "wait-for-period";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string WaitAmount { get; set; }
    public TimeUnit WaitUnits { get; set; }
}