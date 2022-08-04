﻿using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Default.Events;
using Strem.Flows.Default.Flows.Tasks.Data;

namespace Strem.Flows.Default.Flows.Tasks;

public class RaiseEventTask : FlowTask<RaiseEventTaskData>
{
    public static readonly string TaskCode = "raise-event";
    public static readonly string TaskVersion = "1.0.0";
    public override string Code => TaskCode;
    public override string Version => TaskVersion;
    
    public override string Name => "Raise An Event";
    public override string Description => "Raises an event for other flows to listen to";

    public RaiseEventTask(ILogger<IFlowTask> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    public override async Task Execute(RaiseEventTaskData data, IVariables flowVars)
    {
        EventBus.PublishAsync(new UserDataEvent(data.EventName, data.Data));
    }
}