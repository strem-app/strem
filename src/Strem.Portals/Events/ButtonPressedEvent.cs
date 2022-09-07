namespace Strem.Portals.Events;

public record ButtonPressedEvent(Guid PortalId, string PortalName, Guid ButtonId, string ButtonName);