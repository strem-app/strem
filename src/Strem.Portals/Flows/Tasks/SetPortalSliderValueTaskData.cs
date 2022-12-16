using System.ComponentModel.DataAnnotations;
using Strem.Core.Services.Validation.Attributes;
using Strem.Flows.Data.Tasks;
using Strem.Portals.Data;

namespace Strem.Portals.Flows.Tasks;

public class SetPortalSliderValueTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "set-portal-slider-value";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [NotEmpty]
    public Guid PortalId { get; set; }
    [NotEmpty]
    public Guid ElementId { get; set; }
    [Required]
    public string NewValue { get; set; }
}