namespace Strem.Twitch.Services.OAuth;

public interface ITwitchOAuthClient
{
    void StartAuthorisationProcess();
    Task<bool> ValidateToken();
}