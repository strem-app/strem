using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.OBS.Extensions;
using Strem.OBS.Services.Client;

namespace Strem.OBS.Flows.Tasks;

public class SetTextSourceTask : FlowTask<SetTextSourceTaskData>
{
    public override string Code => SetTextSourceTaskData.TaskCode;
    public override string Version => SetTextSourceTaskData.TaskVersion;
    
    public override string Name => "Set Text Source";
    public override string Category => "OBS v4";
    public override string Description => "Changes the text within a text source";

    public IObservableOBSClient ObsClient { get; }

    public SetTextSourceTask(ILogger<FlowTask<SetTextSourceTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSClient obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => ObsClient.IsConnected;

    public override async Task<ExecutionResult> Execute(SetTextSourceTaskData data, IVariables flowVars)
    {
        if(string.IsNullOrEmpty(data.SourceName))
        { return ExecutionResult.Failed(); }

        var textProperties = await ObsClient.GetTextGDIPlusProperties(data.SourceName);
        textProperties.Text = FlowStringProcessor.Process(data.Text, flowVars);
        
        await ObsClient.SetTextGDIPlusProperties(textProperties);
        return ExecutionResult.Success();
    }
}