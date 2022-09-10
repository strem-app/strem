namespace Strem.Portals.Events.Base;

public abstract record PortalButtonEvent(Guid PortalId, Guid ButtonId) : PortalEvent(PortalId);