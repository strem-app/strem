namespace Strem.Portals.Data;

public class PortalStore : IPortalStore
{
    public List<PortalData> Portals { get; set; } = new();

    public PortalStore(IEnumerable<PortalData>? portals = null)
    {
        if(portals != null)
        { Portals.AddRange(portals); }
    }
}