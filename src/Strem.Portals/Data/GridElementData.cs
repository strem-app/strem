using System.ComponentModel.DataAnnotations;
using Strem.Portals.Types;

namespace Strem.Portals.Data;

public class GridElementData
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public GridElementType ElementType { get; set; } = GridElementType.Button;
    public int GridIndex { get; set; }

    [Required] 
    public string Name { get; set; } = string.Empty;
    public ButtonStyles DefaultStyles { get; set; } = new();
}