using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace Strem.Youtube.Services.Client;

public interface IObservableYoutubeClient
{
    YouTubeService YoutubeApiClient { get; }
    PeopleServiceService PeopleApiService { get; }
    Task<Person> GetCurrentUser();
    Task<ICollection<Channel>> GetCurrentUsersChannels();
}