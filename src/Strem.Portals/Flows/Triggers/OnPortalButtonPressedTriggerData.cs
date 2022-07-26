﻿using Strem.Flows.Data.Triggers;
using Strem.Core.Services.Validation.Attributes;

namespace Strem.Portals.Flows.Triggers;

public class OnPortalButtonPressedTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-portal-button-pressed";
    public static readonly string TriggerVersion = "1.0.0";

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;
    
    [NotEmpty]
    public Guid RequiredPortalId { get; set; }
    [NotEmpty]
    public Guid RequiredElementId { get; set; }
}