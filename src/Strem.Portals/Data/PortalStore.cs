namespace Strem.Portals.Data;

public class PortalStore : IPortalStore
{
    public List<PortalData> Portals { get; set; } = new();
}