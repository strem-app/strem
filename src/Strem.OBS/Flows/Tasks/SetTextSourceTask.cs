using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Obs.v5.WebSocket.Reactive;
using Strem.Core.Events.Bus;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Executors;
using Strem.Flows.Processors;

namespace Strem.OBS.Flows.Tasks;

public class SetTextSourceTask : FlowTask<SetTextSourceTaskData>
{
    public override string Code => SetTextSourceTaskData.TaskCode;
    public override string Version => SetTextSourceTaskData.TaskVersion;
    
    public override string Name => "Set Text Source";
    public override string Category => "OBS";
    public override string Description => "Changes the text within a text source";

    public IObservableOBSWebSocket ObsClient { get; }

    public SetTextSourceTask(ILogger<FlowTask<SetTextSourceTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => ObsClient.IsConnected;

    public override async Task<ExecutionResult> Execute(SetTextSourceTaskData data, IVariables flowVars)
    {
        if(string.IsNullOrEmpty(data.SourceName))
        { return ExecutionResult.Failed("SourceName is empty and is required"); }
        
        if(string.IsNullOrEmpty(data.Text))
        { return ExecutionResult.Failed("Text is empty and is required"); }

        var newData = new JObject();
        newData["text"] = FlowStringProcessor.Process(data.Text, flowVars);
        ObsClient.SetInputSettings(data.SourceName, newData);
        return ExecutionResult.Success();
    }
}