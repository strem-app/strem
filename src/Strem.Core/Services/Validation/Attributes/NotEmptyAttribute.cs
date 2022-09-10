using System.ComponentModel.DataAnnotations;

namespace Strem.Core.Services.Validation.Attributes;

[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, 
    AllowMultiple = false)]
public class NotEmptyAttribute : ValidationAttribute
{
    public const string DefaultErrorMessage = "The {0} field must not be empty";
    public NotEmptyAttribute() : base(DefaultErrorMessage) { }

    public override bool IsValid(object? value)
    {
        if (value is null) 
        { return true; }

        return value switch
        {
            Guid guid => guid != Guid.Empty,
            string text => !string.IsNullOrEmpty(text),
            _ => true
        };
    }
}