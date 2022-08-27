using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.OBS.Services.Client;

namespace Strem.OBS.Flows.Tasks;

public class SetBrowserSourceUrlTask : FlowTask<SetBrowserSourceUrlTaskData>
{
    public override string Code => SetBrowserSourceUrlTaskData.TaskCode;
    public override string Version => SetBrowserSourceUrlTaskData.TaskVersion;
    
    public override string Name => "Set Browser Source Url";
    public override string Category => "OBS v4";
    public override string Description => "Changes the url for a browser source";

    public IObservableOBSClient ObsClient { get; }

    public SetBrowserSourceUrlTask(ILogger<FlowTask<SetBrowserSourceUrlTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSClient obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => ObsClient.IsConnected;

    public override async Task<ExecutionResult> Execute(SetBrowserSourceUrlTaskData data, IVariables flowVars)
    {
        if(string.IsNullOrEmpty(data.SourceName))
        { return ExecutionResult.Failed("SourceName is empty and is required"); }
        
        if(string.IsNullOrEmpty(data.Url))
        { return ExecutionResult.Failed("Url is empty and is required"); }

        var browserProperties = await ObsClient.GetBrowserSourceProperties(data.SourceName, null);
        browserProperties.URL = FlowStringProcessor.Process(data.Url, flowVars);
        
        await ObsClient.SetBrowserSourceProperties(data.SourceName, browserProperties, null);
        return ExecutionResult.Success();
    }
}