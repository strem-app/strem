using Strem.Portals.Events.Base;

namespace Strem.Portals.Events;

public record PortalChangedEvent(Guid PortalId) : PortalEvent(PortalId);