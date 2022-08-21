namespace Strem.Core.Portals;

public class PortalStore : IPortalStore
{
    public List<PortalData> Portals { get; set; } = new();
}