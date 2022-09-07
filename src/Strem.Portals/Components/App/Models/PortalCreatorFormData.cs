using System.ComponentModel.DataAnnotations;

namespace Strem.Portals.Components.App.Models;

public class PortalCreatorFormData
{
    [Required] public string Name { get; set; } = string.Empty;
}