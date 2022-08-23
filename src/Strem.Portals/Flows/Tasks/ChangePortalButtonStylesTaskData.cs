using Strem.Core.Flows.Tasks;
using Strem.Portals.Data;

namespace Strem.Portals.Flows.Tasks;

public class ChangePortalButtonStylesTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "change-portal-button-style";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public Guid PortalId { get; set; }
    public Guid ButtonId { get; set; }
    
    public bool ChangeText { get; set; }
    public bool ChangeIcon { get; set; }
    public bool ChangeImage { get; set; }
    public bool ChangeButtonType { get; set; }
    public bool ChangeBackgroundColor { get; set; }
    public bool ChangeTextColor { get; set; }

    public ButtonStyles NewStyles { get; set; } = new ButtonStyles
    {
        IconClass = string.Empty,
        ImageUrl = string.Empty
    };
}