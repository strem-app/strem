using Strem.Core.Events.Bus;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Twitter.Extensions;
using Strem.Twitter.Types;

namespace Strem.Twitter.Flow.Tasks;

public class SendTweetTask : FlowTask<SendTweetTaskData>
{
    public override string Code => SendTweetTaskData.TaskCode;
    public override string Version => SendTweetTaskData.TaskVersion;
    
    public override string Name => "Send A Tweet";
    public override string Category => "Twitter";
    public override string Description => "Sends a tweet";
    
    public SendTweetTask(ILogger<FlowTask<SendTweetTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    public override bool CanExecute() => AppState.HasTwitterOAuth() && AppState.HasTwitterScope(Scopes.WriteTweet);

    public override async Task<ExecutionResult> Execute(SendTweetTaskData data, IVariables flowVars)
    {
        var processedTweet = FlowStringProcessor.Process(data.TweetContent, flowVars);
        return ExecutionResult.Success();
    }
}