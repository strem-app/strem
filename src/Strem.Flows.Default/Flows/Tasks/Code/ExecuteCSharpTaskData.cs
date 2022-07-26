using Strem.Flows.Data.Tasks;
using Strem.Flows.Default.Execution;

namespace Strem.Flows.Default.Flows.Tasks.Code;

public class ExecuteCSharpTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "execute-csharp";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;

    public string CSharpCode { get; set; } = DefaultCodeGen.DefaultCSharpCodegen;
}