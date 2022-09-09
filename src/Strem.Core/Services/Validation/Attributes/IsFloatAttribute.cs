using System.ComponentModel.DataAnnotations;

namespace Strem.Core.Services.Validation.Attributes;

public class IsFloatAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null) { return ValidationResult.Success;}
        
        var isNumeric = float.TryParse(value.ToString(), out var val);
        return !isNumeric ? new ValidationResult("Must be a numeric value") : ValidationResult.Success;
    }
}