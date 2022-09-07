using System.ComponentModel.DataAnnotations;

namespace Strem.UI.Components.Flows.Models;

public class FlowCreatorFormData
{
    [Required] 
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}