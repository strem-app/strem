using Google.Apis.YouTube.v3;

namespace Strem.Youtube.Services.Client;

public class ObservableYoutubeClient : IObservableYoutubeClient
{
    public YouTubeService ApiClient { get; }

    public ObservableYoutubeClient(YouTubeService youtubeClient)
    {
        ApiClient = youtubeClient;
    }

    public void SetupObservables()
    {
       
    }
}