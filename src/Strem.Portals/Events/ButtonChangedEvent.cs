using Strem.Portals.Events.Base;

namespace Strem.Portals.Events;

public record ButtonChangedEvent(Guid PortalId, Guid ButtonId) : PortalButtonEvent(PortalId, ButtonId);