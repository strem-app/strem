using Strem.Portals.Events.Base;

namespace Strem.Portals.Events;

public record PortalElementPressedEvent(Guid PortalId, string PortalName, Guid ElementId, string ButtonName) : PortalElementEvent(PortalId, ElementId);