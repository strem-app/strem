namespace Strem.Infrastructure.Services.Files;

public interface IFileBrowser
{
    string BrowseForFile(string startingDirectory);
}