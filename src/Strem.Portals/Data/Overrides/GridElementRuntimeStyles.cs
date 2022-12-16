namespace Strem.Portals.Data.Overrides;

public class GridElementRuntimeStyles
{
    public Dictionary<Guid, Dictionary<Guid, ElementStyles>> RuntimeStyles { get; set; } = new();
}