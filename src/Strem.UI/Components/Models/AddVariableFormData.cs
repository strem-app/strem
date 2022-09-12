using System.ComponentModel.DataAnnotations;

namespace Strem.UI.Components.Models;

public class AddVariableFormData
{
    [Required]
    public string Name { get; set; }
    
    public string Context { get; set; }
    
    [Required]
    public string Value { get; set; }
}