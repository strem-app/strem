using Strem.Core.Extensions;

namespace Strem.Flows.Default.Execution;

public static class DefaultCodeGen
{
    public static string DefaultCSharpCodegen = $@"namespace CustomCSharpCode;

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

    public static string DefaultPythonCodegen = $@"import clr
clr.AddReference(""Strem.Core"")
from Strem.Core import Extensions
clr.ImportExtensions(Extensions)

def execute(context):
    # Your Code Goes Here";
    
    public static string DefaultPowershellCodegen = $@"# Your code goes here";
    
}