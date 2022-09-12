using Strem.Portals.Data;

namespace Strem.Portals.Services.Data;

public record PortalDataWrapper(IReadOnlyCollection<PortalData> Portals);