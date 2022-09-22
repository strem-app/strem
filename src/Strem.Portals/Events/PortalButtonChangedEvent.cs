using Strem.Portals.Events.Base;

namespace Strem.Portals.Events;

public record PortalButtonChangedEvent(Guid PortalId, Guid ButtonId) : PortalButtonEvent(PortalId, ButtonId);