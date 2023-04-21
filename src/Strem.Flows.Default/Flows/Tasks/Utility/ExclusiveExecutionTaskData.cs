using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Strem.Core.Types;
using Strem.Flows.Data;
using Strem.Flows.Data.Tasks;

namespace Strem.Flows.Default.Flows.Tasks.Utility;

public class ExclusiveExecutionTaskData : IFlowTaskData, IHasSubTaskData
{
    public static readonly string TaskCode = "exclusive-execution";
    public static readonly string TaskVersion = "1.0.0";
    public static readonly string SubTaskKey = "sub-tasks";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public string GroupName { get; set; } = string.Empty;
    [Required]
    public string TimeoutAmount { get; set; } = "10";
    [Required] 
    public TimeUnitType TimeoutUnitsType { get; set; } = TimeUnitType.Seconds;

    [JsonIgnore]
    public string[] SubTaskKeys { get; } = new[] { SubTaskKey };
    public Dictionary<string, List<IFlowTaskData>> SubTasks { get; set; }

    public ExclusiveExecutionTaskData()
    { SubTasks ??= SubTaskKeys.ToDictionary(x => x, x => new List<IFlowTaskData>()); }
}