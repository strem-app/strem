using System.ComponentModel.DataAnnotations;
using Strem.Core.Services.Validation.Attributes;

namespace Strem.UI.Components.Models;

public class AddVariableFormData
{
    [Required]
    [IsVariablePattern]
    public string Name { get; set; }
    
    [IsVariablePattern]
    public string Context { get; set; }
    
    [Required]
    public string Value { get; set; }
}