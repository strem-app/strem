using Strem.Portals.Events.Base;

namespace Strem.Portals.Events;

public record PortalCreatedEvent(Guid PortalId) : PortalEvent(PortalId);