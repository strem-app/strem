﻿using Strem.Core.Flows.Tasks;
using Strem.Core.Types;

namespace Strem.Flows.Default.Flows.Tasks.Data;

public class SetVariableTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "set-variable";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public string Name { get; set; }
    public string Context { get; set; }
    public string Value { get; set; }
    public VariableScope Scope { get; set; }
}