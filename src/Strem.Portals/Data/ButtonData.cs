
using Newtonsoft.Json;

namespace Strem.Portals.Data;

public class ButtonData
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public ButtonStyles DefaultStyles { get; set; } = new();
}