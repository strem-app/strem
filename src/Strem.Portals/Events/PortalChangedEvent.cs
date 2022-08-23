namespace Strem.Portals.Events;

public class PortalChangedEvent
{
    public Guid PortalId { get; set; }
    public string PortalName { get; set; }
}