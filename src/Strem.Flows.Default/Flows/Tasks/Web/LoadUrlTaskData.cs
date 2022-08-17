using RestSharp;
using Strem.Core.Flows.Tasks;

namespace Strem.Flows.Default.Flows.Tasks.Web;

public class LoadUrlTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "load-url-flow";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string Url { get; set; } = string.Empty;
}