using System.ComponentModel.DataAnnotations;

namespace Strem.Core.Services.Validation.Attributes;

public class IsIntegerAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null) { return ValidationResult.Success;}
        
        var isNumeric = int.TryParse(value.ToString(), out var val);
        return !isNumeric ? new ValidationResult("Must be a numeric value without decimal points") : ValidationResult.Success;
    }
}