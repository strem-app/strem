using System.Net.Http.Headers;
using Google.Apis.Http;
using Strem.Core.State;
using Strem.Youtube.Extensions;

namespace Strem.Youtube.Services.Client;

public class AccessTokenUserCredential : IHttpExecuteInterceptor, IConfigurableHttpClientInitializer
{
    public IAppState AppState { get; }

    public AccessTokenUserCredential(IAppState appState)
    {
        AppState = appState;
    }

    public void Initialize(ConfigurableHttpClient httpClient)
    {
        httpClient.MessageHandler.AddExecuteInterceptor(this);
    }

    public async Task InterceptAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = string.Empty;
        if (AppState.HasYoutubeAccessToken())
        { accessToken = AppState.GetYoutubeAccessToken(); }
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
}