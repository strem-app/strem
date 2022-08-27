using Strem.Core.Types;

namespace Strem.Core.Flows.Executors;

public class ExecutionResult
{
    public ExecutionResultType ResultType { get; }
    public string ExecutionMessage { get; }
    public string[] SubTaskKeys { get; }

    public ExecutionResult(ExecutionResultType resultType, string executionMessage, params string[] subTaskKey)
    {
        ResultType = resultType;
        ExecutionMessage = executionMessage;
        SubTaskKeys = subTaskKey;
    }

    public static ExecutionResult Success(params string[] subTaskKeys) => new(ExecutionResultType.Success, string.Empty, subTaskKeys);
    public static ExecutionResult Failed(string message, params string[] subTaskKeys) => new(ExecutionResultType.Failed, message, subTaskKeys);
    public static ExecutionResult CascadingFailure(string message, params string[] subTaskKeys) => new(ExecutionResultType.CascadingFailure, message, subTaskKeys);
    public static ExecutionResult FailedButContinue(string message, params string[] subTaskKeys) => new(ExecutionResultType.FailedButContinue, message, subTaskKeys);
}