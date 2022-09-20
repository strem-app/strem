using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Data;

namespace Strem.Flows.Default.Flows.Tasks.Logic;

public class RandomSubFlowTaskData : IFlowTaskData, IHasSubTaskData
{
    public static readonly string TaskCode = "random-sub-flow";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public List<string> SubFlowNames { get; set; } = new();

    [JsonIgnore]
    public string[] SubTaskKeys => SubFlowNames.ToArray();
    public Dictionary<string, List<IFlowTaskData>> SubTasks { get; set; }

    public RandomSubFlowTaskData()
    { SubTasks ??= SubTaskKeys.ToDictionary(x => x, x => new List<IFlowTaskData>()); }
}