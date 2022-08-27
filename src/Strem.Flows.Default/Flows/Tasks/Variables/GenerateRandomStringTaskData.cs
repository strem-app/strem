using Strem.Core.Flows.Tasks;
using Strem.Core.Types;

namespace Strem.Flows.Default.Flows.Tasks.Variables;

public class GenerateRandomStringTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "generate-random-string";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;

    public string Name { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public VariableScopeType ScopeType { get; set; }

    public string StringLength { get; set; }
}