using Strem.Flows.Data.Tasks;
using Strem.Core.Services.Validation.Attributes;
using Strem.Portals.Data;

namespace Strem.Portals.Flows.Tasks;

public class ChangePortalButtonStylesTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "change-portal-button-style";
    public static readonly string TaskVersion = "1.1.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [NotEmpty]
    public Guid PortalId { get; set; }
    [NotEmpty]
    public Guid ButtonId { get; set; }
    
    public bool ChangeText { get; set; }
    public bool ChangeIcon { get; set; }
    public bool ChangeImage { get; set; }
    public bool ChangeButtonType { get; set; }
    public bool ChangeBackgroundColor { get; set; }
    public bool ChangeTextColor { get; set; }

    public ElementStyles NewStyles { get; set; } = new();
}