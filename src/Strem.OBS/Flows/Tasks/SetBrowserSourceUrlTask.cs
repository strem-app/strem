﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Obs.v5.WebSocket.Reactive;
using Strem.Core.Events.Bus;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.OBS.Extensions;

namespace Strem.OBS.Flows.Tasks;

public class SetBrowserSourceUrlTask : FlowTask<SetBrowserSourceUrlTaskData>
{
    public override string Code => SetBrowserSourceUrlTaskData.TaskCode;
    public override string Version => SetBrowserSourceUrlTaskData.TaskVersion;
    
    public override string Name => "Set Browser Source Url";
    public override string Category => "OBS";
    public override string Description => "Changes the url for a browser source";

    public IObservableOBSWebSocket ObsClient { get; }

    public SetBrowserSourceUrlTask(ILogger<FlowTask<SetBrowserSourceUrlTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => ObsClient.IsConnected;

    public override async Task<ExecutionResult> Execute(SetBrowserSourceUrlTaskData data, IVariables flowVars)
    {
        var newData = new JObject { ["url"] = FlowStringProcessor.Process(data.Url, flowVars) };
        ObsClient.SetInputSettings(data.BrowserSourceName, newData);
        return ExecutionResult.Success();
    }
}