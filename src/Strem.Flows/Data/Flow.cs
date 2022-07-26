﻿using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Data.Triggers;

namespace Strem.Flows.Data;

public class Flow
{
    public string Version { get; set; } = "1.0.0";
    
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    public string Category { get; set; }
    public bool Enabled { get; set; }
    
    public List<IFlowTriggerData> TriggerData { get; set; } = new();
    public List<IFlowTaskData> TaskData { get; set; } = new();

    public Flow(Guid id, string name, string category)
    {
        Id = id;
        Name = name;
        Category = category;
    }

    public Flow()
    {
    }
}