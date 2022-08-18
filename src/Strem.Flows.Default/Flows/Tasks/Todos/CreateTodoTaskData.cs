﻿using Strem.Core.Flows.Tasks;
using Strem.Core.Todo;
using Strem.Core.Types;

namespace Strem.Flows.Default.Flows.Tasks.Todos;

public class CreateTodoTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "create-todo";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string Title { get; set; }
    public string CreatedBy { get; set; }
    public TimeUnitType TimeoutType { get; set; }
    public string TimeoutValue { get; set; }
    public string Payload { get; set; }
    public TodoActionType ActionType { get; set; }
}