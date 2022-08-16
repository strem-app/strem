using RestSharp;
using Strem.Core.Flows.Tasks;
using Strem.Flows.Default.Models;

namespace Strem.Flows.Default.Flows.Tasks.Web;

public class MakeHttpRequestTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "make-http-request";
    public static readonly string TaskVersion = "1.0.0";

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;

    public string Url { get; set; }
    public Method Verb { get; set; }
    public string Body { get; set; }
    public DataFormat DataFormat { get; set; }
    public List<HeaderData> Headers { get; set; } = new();
}