namespace Strem.Core.Services.Validation;

public interface IDataValidator
{
    DataValidationResult Validate<T>(T instance) where T : class;
}