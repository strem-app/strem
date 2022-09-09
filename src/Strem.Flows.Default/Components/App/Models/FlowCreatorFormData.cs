using System.ComponentModel.DataAnnotations;

namespace Strem.Flows.Default.Components.App.Models;

public class FlowCreatorFormData
{
    [Required] 
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}