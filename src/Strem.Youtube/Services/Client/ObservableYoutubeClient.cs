using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace Strem.Youtube.Services.Client;

public class ObservableYoutubeClient : IObservableYoutubeClient
{
    public YouTubeService YoutubeApiClient { get; }
    public PeopleServiceService PeopleApiService { get; }
    
    public DateTime LastChatUpdateTime { get; set; }
    
    /// <summary>Occurs when [on channel state changed].</summary>
    public IObservable<LiveChatMessage> OnChannelStateChanged { get; private set; }

    public ObservableYoutubeClient(YouTubeService youtubeClient, PeopleServiceService peopleClient)
    {
        YoutubeApiClient = youtubeClient;
        PeopleApiService = peopleClient;
    }

    public void SetupObservables()
    {
       //YoutubeApiClient.LiveStreams.
    }

    public Task<Person> GetCurrentUser()
    {
        var personRequest = PeopleApiService.People.Get("people/me");
        personRequest.PersonFields = "names";
        return personRequest.ExecuteAsync();
    }

    public async Task<ICollection<Channel>> GetCurrentUsersChannels()
    {
        var channelRequest = YoutubeApiClient.Channels.List("id, snippet");
        channelRequest.Mine = true;
        var result = await channelRequest.ExecuteAsync();
        return result.Items;
    }

    public void StartListeningForChatMessagesOn(string liveChatId)
    {
        LastChatUpdateTime = new DateTime();
        var request = YoutubeApiClient.LiveChatMessages.List(liveChatId, "id, snippet");
        var a = request.Execute();
        
    }
}