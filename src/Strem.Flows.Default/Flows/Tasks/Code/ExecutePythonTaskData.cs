using Strem.Flows.Data.Tasks;
using Strem.Flows.Default.Execution;

namespace Strem.Flows.Default.Flows.Tasks.Code;

public class ExecutePythonTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "execute-python";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;

    public string PythonCode { get; set; } = DefaultCodeGen.DefaultPythonCodegen;
}