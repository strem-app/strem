using Strem.Core.Services.Browsers.File;

namespace Strem.Core.Extensions;

public static class IFileBrowserExtensions
{
    public static string VerifyPathEnd(string path, string fileExtension)
    {
        if(string.IsNullOrEmpty(path)) 
        { return string.Empty; }

        if (!Path.HasExtension(path))
        { path = $"{path}.{fileExtension}"; }

        return path;
    }
    
    public static string BrowseToOpenJsonFile(this IFileBrowser browser)
    {
        return browser.BrowseToOpenFile(filterList: "json");
    }
    
    public static string BrowseToSaveJsonFile(this IFileBrowser browser)
    {
        var filePath = browser.BrowseToSaveFile(filterList: "json");
        return VerifyPathEnd(filePath, "json");
    }
}