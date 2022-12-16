using Strem.Portals.Events.Base;

namespace Strem.Portals.Events;

public record PortalSliderValueChangedEvent(Guid PortalId, string PortalName, Guid ElementId, string ElementName, int Value) : PortalElementEvent(PortalId, ElementId);