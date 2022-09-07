using System.ComponentModel.DataAnnotations;

namespace Strem.Portals.Data;

public class ButtonData
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required] 
    public string Name { get; set; } = string.Empty;
    public ButtonStyles DefaultStyles { get; set; } = new();
}