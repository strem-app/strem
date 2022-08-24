using Strem.Core.Types;

namespace Strem.Core.Flows.Executors;

public class ExecutionResult
{
    public ExecutionResultType ResultType { get; }
    public string[] SubTaskKeys { get; }

    public ExecutionResult(ExecutionResultType resultType, params string[] subTaskKey)
    {
        ResultType = resultType;
        SubTaskKeys = subTaskKey;
    }

    public static ExecutionResult Success(params string[] subTaskKeys) => new(ExecutionResultType.Success, subTaskKeys);
    public static ExecutionResult Failed(params string[] subTaskKeys) => new(ExecutionResultType.Failed, subTaskKeys);
    public static ExecutionResult CascadingFailure(params string[] subTaskKeys) => new(ExecutionResultType.CascadingFailure, subTaskKeys);
    public static ExecutionResult FailedButContinue(params string[] subTaskKeys) => new(ExecutionResultType.FailedButContinue, subTaskKeys);
}