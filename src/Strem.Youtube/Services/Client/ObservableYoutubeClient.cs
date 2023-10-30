using Google.Apis.YouTube.v3;

namespace Strem.Youtube.Services.Client;

public class ObservableYoutubeClient : IObservableYoutubeClient
{
    public YouTubeService YouTubeService { get; }

    public ObservableYoutubeClient(YouTubeService youTubeService)
    {
        YouTubeService = youTubeService;
    }
    
    public void SetupObservables()
    {
       
    }
}