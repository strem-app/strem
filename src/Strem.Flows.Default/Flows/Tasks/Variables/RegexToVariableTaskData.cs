using Strem.Core.Flows.Tasks;
using Strem.Core.Types;

namespace Strem.Flows.Default.Flows.Tasks.Variables;

public class RegexToVariableTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "regex-to-variable";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string Source { get; set; }
    public string Pattern { get; set; }
    
    public string Name { get; set; }
    public string Context { get; set; }
    public VariableScopeType ScopeType { get; set; }
}