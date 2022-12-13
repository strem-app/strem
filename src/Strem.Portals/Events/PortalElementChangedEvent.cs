using Strem.Portals.Events.Base;

namespace Strem.Portals.Events;

public record PortalElementChangedEvent(Guid PortalId, Guid ButtonId) : PortalButtonEvent(PortalId, ButtonId);