using Google.Apis.YouTube.v3;

namespace Strem.Youtube.Services.Client;

public interface IObservableYoutubeClient
{
    YouTubeService ApiClient { get; }
}