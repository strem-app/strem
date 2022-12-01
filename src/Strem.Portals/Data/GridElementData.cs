using System.ComponentModel.DataAnnotations;

namespace Strem.Portals.Data;

public class GridElementData
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int GridIndex { get; set; }

    [Required] 
    public string Name { get; set; } = string.Empty;
    public ElementStyles DefaultStyles { get; set; } = new ();
}