using Strem.Core.Extensions;

namespace Strem.Flows.Default.Execution;

public abstract class MethodExecutor
{
    public abstract Task Execute(ExecutionContext context);
}

public static class DefaultCodeGen
{
    public static string DefaultMethodExecutor = $@"namespace CustomCSharpCode;

using {typeof(MethodExecutor).Namespace};
using {typeof(IVariablesExtensions).Namespace};
using {typeof(Task).Namespace};

public class Executor : MethodExecutor
{{
    public async override Task Execute(ExecutionContext context)
    {{
        // Your code goes here
    }}
}}";
        
}