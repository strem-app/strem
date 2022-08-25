namespace Strem.Portals.Events;

public class ButtonPressedEvent
{
    public Guid PortalId { get; set; }
    public string PortalName { get; set; }
    public Guid ButtonId { get; set; }
    public string ButtonName { get; set; }
}