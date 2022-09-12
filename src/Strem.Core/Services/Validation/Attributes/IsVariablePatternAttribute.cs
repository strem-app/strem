using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Strem.Core.Variables;

namespace Strem.Core.Services.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class IsVariablePatternAttribute : ValidationAttribute
{
    public const string DefaultErrorMessage = "The {0} field can only contain alphanumeric and . - _ | : characters";
    public static Regex ContextPattern = new Regex($@"[{CommonVariables.VariableNamingPattern}]");
    
    public IsVariablePatternAttribute() : base(DefaultErrorMessage) { }

    public override bool IsValid(object? value)
    {
        if (value is null) { return true; }

        var valueString = value.ToString();
        return string.IsNullOrEmpty(valueString) || ContextPattern.IsMatch(valueString);
    }
}