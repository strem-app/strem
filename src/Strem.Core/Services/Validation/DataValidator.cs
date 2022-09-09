using System.ComponentModel.DataAnnotations;

namespace Strem.Core.Services.Validation;

public class DataValidator : IDataValidator
{
    public DataValidationResult Validate<T>(T instance) where T : class
    {
        var context = new ValidationContext(instance);
        var listErrors = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(instance, context, listErrors, true);

        return new DataValidationResult(isValid, listErrors
            .Select(x => $"{string.Join(",", x.MemberNames)} : {x.ErrorMessage}")
            .ToArray());
    }
}