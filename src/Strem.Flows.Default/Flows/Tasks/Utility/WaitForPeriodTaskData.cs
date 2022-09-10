using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;
using Strem.Core.Types;

namespace Strem.Flows.Default.Flows.Tasks.Utility;

public class WaitForPeriodTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "wait-for-period";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public string WaitAmount { get; set; } = string.Empty;
    [Required]
    public TimeUnitType WaitUnitsType { get; set; }
}