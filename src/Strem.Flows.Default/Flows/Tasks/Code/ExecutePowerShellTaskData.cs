using Strem.Flows.Data.Tasks;
using Strem.Flows.Default.Execution;

namespace Strem.Flows.Default.Flows.Tasks.Code;

public class ExecutePowerShellTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "execute-powershell";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;

    public string PSCode { get; set; } = DefaultCodeGen.DefaultPowershellCodegen;
}