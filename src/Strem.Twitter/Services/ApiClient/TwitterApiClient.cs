using Newtonsoft.Json.Linq;
using RestSharp;
using Strem.Core.State;
using Strem.Twitter.Extensions;

namespace Strem.Twitter.Services.ApiClient;

public class TwitterApiClient : ITwitterApiClient
{
    private const string TwitterV2Api = "https://api.twitter.com/2";
    private const string TweetEndpoint = "tweets";
    
    public IAppState AppState { get; set; }

    public TwitterApiClient(IAppState appState)
    {
        AppState = appState;
    }

    public async Task<(string Id, string Text)> MakeTweet(string tweetContent)
    {
        if (!AppState.HasTwitterOAuth())
        { throw new Exception("No OAuth Token available for making twitter request"); }
        
        var restClient = new RestClient(TwitterV2Api);
        var restRequest = new RestRequest(TweetEndpoint, Method.Post);
        restRequest.AddHeader("authorization", $"bearer {AppState.GetTwitterOAuthToken()}");
        restRequest.AddJsonBody(new { text = tweetContent });
        
        var response = await restClient.ExecuteAsync(restRequest);
        if(!response.IsSuccessful) 
        { throw new Exception($"Twitter Query Failed: {response.StatusCode}:{response.Content}"); }

        var json = JObject.Parse(response.Content);
        return new() { Id = json["data"]["id"].ToString(), Text = json["data"]["text"].ToString() };
    }
}