using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;

namespace Strem.OBS.Flows.Tasks;

public class SetBrowserSourceUrlTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "set-obs-browser-source-url";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public string BrowserSourceName { get; set; }
    
    [Required]
    public string Url { get; set; }
}