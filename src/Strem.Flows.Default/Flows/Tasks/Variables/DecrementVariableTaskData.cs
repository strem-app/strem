using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;
using Strem.Core.Types;

namespace Strem.Flows.Default.Flows.Tasks.Variables;

public class DecrementVariableTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "decrement-variable";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    [Required]
    public VariableScopeType ScopeType { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "{0} Must Be A Positive Number")]
    public int DecrementAmount { get; set; } = 1;
}