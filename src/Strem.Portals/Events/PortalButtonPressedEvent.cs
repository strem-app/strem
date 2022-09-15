using Strem.Portals.Events.Base;

namespace Strem.Portals.Events;

public record PortalButtonPressedEvent(Guid PortalId, string PortalName, Guid ButtonId, string ButtonName) : PortalButtonEvent(PortalId, ButtonId);