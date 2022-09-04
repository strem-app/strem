namespace Strem.Core.Browsers.File;

public interface IFileBrowser
{
    string BrowseForFile(string startingDirectory = null);
}