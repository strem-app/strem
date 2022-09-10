namespace Strem.Core.Services.Browsers.File;

public interface IFileBrowser
{
    string BrowseForFile(string startingDirectory = null);
}