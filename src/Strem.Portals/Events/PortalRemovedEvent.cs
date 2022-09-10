using Strem.Portals.Events.Base;

namespace Strem.Portals.Events;

public record PortalRemovedEvent(Guid PortalId) : PortalEvent(PortalId);