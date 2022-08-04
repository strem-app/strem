using Strem.Core.Flows.Tasks;
using Strem.Core.Types;
using Strem.Core.Utils;

namespace Strem.Flows.Default.Flows.Tasks.Data;

public class WaitForPeriodTaskData : IFlowTaskData
{
    public string Code => WaitForPeriodTask.TaskCode;
    public string Version { get; set; } = WaitForPeriodTask.TaskVersion;
    
    public string WaitAmount { get; set; }
    public TimeUnit WaitUnits { get; set; }
}