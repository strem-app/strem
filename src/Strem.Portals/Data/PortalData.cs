using System.ComponentModel.DataAnnotations;

namespace Strem.Portals.Data;

public class PortalData
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Version { get; set; } = "1.0.0";
    
    [Required]
    public string Name { get; set; }
    public string Password { get; set; }
    
    public bool ShowTodos { get; set; }
    public List<string> TodoTags { get; set; } = new();

    [Range(1, 100, ErrorMessage = "{0} must be between 1 and 100 in size")]
    public int ElementGridSize { get; set; } = 20;
    
    public List<GridElementData> Elements { get; set; } = new();

    public PortalData(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public PortalData()
    {
    }
}

