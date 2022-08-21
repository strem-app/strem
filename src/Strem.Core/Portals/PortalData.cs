using Strem.Core.Types;

namespace Strem.Core.Portals;

public class PortalData
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ButtonSize ButtonSize { get; set; }
    public List<ButtonData> Buttons { get; set; } = new();

    public PortalData(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public PortalData()
    {
    }
}