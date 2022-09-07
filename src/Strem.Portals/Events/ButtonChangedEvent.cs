namespace Strem.Portals.Events;

public record ButtonChangedEvent(Guid PortalId, Guid ButtonId);