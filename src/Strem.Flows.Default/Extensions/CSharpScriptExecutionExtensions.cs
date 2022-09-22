using Strem.Flows.Default.Execution;
using Westwind.Scripting;

namespace Strem.Flows.Default.Extensions;

public static class CSharpScriptExecutionExtensions
{
    public static MethodExecutor CompileMethodExecutor(this CSharpScriptExecution scriptExecutor, string code)
    {
        scriptExecutor.AddDefaultReferencesAndNamespaces();
        scriptExecutor.AddLoadedReferences();
        return (MethodExecutor)scriptExecutor.CompileClass(code);
    }
}