namespace Strem.Twitter.Services.ApiClient;

public interface ITwitterApiClient
{
    Task<(string Id, string Text)> MakeTweet(string tweetContent);
    Task<(string Id, string Username)> GetCurrentUser();
}