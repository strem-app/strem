namespace Strem.Core.Services.Browsers.File;

public interface IFileBrowser
{
    string BrowseToOpenFile(string startingDirectory = null, string filterList = null);
    string BrowseToSaveFile(string startingDirectory = null, string filterList = null);
}