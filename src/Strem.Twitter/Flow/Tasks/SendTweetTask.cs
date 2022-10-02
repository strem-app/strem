using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Twitter.Extensions;
using Strem.Twitter.Plugin;
using Strem.Twitter.Services.ApiClient;
using Strem.Twitter.Types;
using Strem.Twitter.Variables;

namespace Strem.Twitter.Flow.Tasks;

public class SendTweetTask : FlowTask<SendTweetTaskData>
{
    public override string Code => SendTweetTaskData.TaskCode;
    public override string Version => SendTweetTaskData.TaskVersion;
    
    public override string Name => "Send A Tweet";
    public override string Category => "Twitter";
    public override string Description => "Sends a tweet";

    public static readonly VariableEntry TweetIdVariable = new("tweet.id", TwitterVars.Context);
    public static readonly VariableEntry TweetUrlVariable = new("tweet.url", TwitterVars.Context);
    
    public ITwitterApiClient TwitterClient { get; }

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        TweetIdVariable.ToDescriptor(), TweetUrlVariable.ToDescriptor()
    };

    public SendTweetTask(ILogger<FlowTask<SendTweetTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitterApiClient twitterClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitterClient = twitterClient;
    }

    public override bool CanExecute() => AppState.HasTwitterOAuth() && AppState.HasTwitterScopes(Scopes.WriteTweet, Scopes.ReadTweet, Scopes.ReadUsers);

    public void PopulateVariables(string tweetId, IVariables flowVars)
    {
        flowVars.Set(TweetIdVariable, tweetId);
        flowVars.Set(TweetUrlVariable, $"{TwitterPluginSettings.TweetUrlForId}/{tweetId}");
    }
    
    public override async Task<ExecutionResult> Execute(SendTweetTaskData data, IVariables flowVars)
    {
        var processedTweet = FlowStringProcessor.Process(data.TweetContent, flowVars);
        var result = await TwitterClient.MakeTweet(processedTweet);
        PopulateVariables(result.Id, flowVars);
        return ExecutionResult.Success();
    }
}