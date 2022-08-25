namespace Strem.Core.Events.Flows;

public class PortalRemovedEvent
{
    public Guid PortalId { get; }

    public PortalRemovedEvent(Guid portalId)
    {
        PortalId = portalId;
    }
}