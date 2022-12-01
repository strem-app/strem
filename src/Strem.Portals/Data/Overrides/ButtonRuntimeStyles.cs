namespace Strem.Portals.Data.Overrides;

public class ButtonRuntimeStyles
{
    public Dictionary<Guid, Dictionary<Guid, ElementStyles>> RuntimeStyles { get; set; } = new();
}