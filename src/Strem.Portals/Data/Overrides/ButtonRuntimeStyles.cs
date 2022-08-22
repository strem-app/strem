namespace Strem.Portals.Data.Overrides;

public class ButtonRuntimeStyles
{
    public Dictionary<Guid, Dictionary<Guid, ButtonStyles>> RuntimeStyles { get; set; } = new();
}