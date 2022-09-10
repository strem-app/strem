using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;
using Strem.Core.Services.Validation.Attributes;
using Strem.Core.Types;
using Strem.Todos.Data;

namespace Strem.Todos.Flows.Tasks;

public class CreateTodoTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "create-todo";
    public static readonly string TaskVersion = "1.0.1";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public string Title { get; set; }
    [Required]
    public string CreatedBy { get; set; }
    public TimeUnitType TimeoutType { get; set; }
    [Required]
    public string TimeoutValue { get; set; }
    public List<string> Tags { get; set; } = new();
    public TodoActionType ActionType { get; set; }
    [RequiredIf("ActionType", TodoActionType.Text, IsInverted = true)]
    public string Payload { get; set; } = string.Empty;
}