namespace Strem.Flows.Default.Execution;

public abstract class MethodExecutor
{
    public abstract Task Execute(ExecutionContext context);
}