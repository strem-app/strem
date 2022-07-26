﻿using System.ComponentModel.DataAnnotations;
using Strem.Flows.Data.Tasks;
using Strem.OBS.Types;

namespace Strem.OBS.Flows.Tasks;

public class SetSourceMuteStateTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "set-obs-source-mute-state";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;

    [Required]
    public string SourceName { get; set; }
    
    [Required]
    public MuteStatus Status { get; set; }
}