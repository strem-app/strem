namespace Strem.Core.Validation;

public interface IDataValidator
{
    DataValidationResult Validate<T>(T instance) where T : class;
}