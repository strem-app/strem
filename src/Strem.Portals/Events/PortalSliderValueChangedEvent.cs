using Strem.Portals.Events.Base;

namespace Strem.Portals.Events;

public record PortalSliderValueChangedEvent(Guid PortalId, string PortalName, Guid ButtonId, string ButtonName, int Value) : PortalButtonEvent(PortalId, ButtonId);