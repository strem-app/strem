using System.ComponentModel.DataAnnotations;
using RestSharp;
using Strem.Flows.Data.Tasks;

namespace Strem.Flows.Default.Flows.Tasks.Web;

public class LoadUrlTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "load-url-flow";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public string Url { get; set; } = string.Empty;
}