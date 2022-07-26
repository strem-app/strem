﻿using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;

namespace Strem.Flows.Default.Flows.Tasks.Flows;

public class ExecuteFlowTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "execute-flow";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    [Required]
    public Guid FlowId { get; set; }
    public bool WaitForCompletion { get; set; }
}