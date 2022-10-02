namespace Strem.Twitter.Services.ApiClient;

public interface ITwitterApiClient
{
    Task<(string Id, string Text)> MakeTweet(string tweetContent);
}