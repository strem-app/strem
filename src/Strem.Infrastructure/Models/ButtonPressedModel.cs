namespace Strem.Infrastructure.Models;

public class ButtonPressedModel
{
    public Guid PortalId { get; set; }
    public string PortalName { get; set; }
    public Guid ButtonId { get; set; }
    public string ButtonName { get; set; }
}