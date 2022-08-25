namespace Strem.Portals.Events;

public class ButtonChangedEvent
{
    public Guid PortalId { get; set; }
    public Guid ButtonId { get; set; }
}