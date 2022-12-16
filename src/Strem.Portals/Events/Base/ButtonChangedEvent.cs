namespace Strem.Portals.Events.Base;

public abstract record PortalElementEvent(Guid PortalId, Guid ElementId) : PortalEvent(PortalId);