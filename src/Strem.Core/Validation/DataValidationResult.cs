namespace Strem.Core.Validation;

public class DataValidationResult
{
    public bool IsValid { get; }
    public string[] Errors { get; }

    public DataValidationResult(bool isValid, string[] errors)
    {
        IsValid = isValid;
        Errors = errors;
    }
}