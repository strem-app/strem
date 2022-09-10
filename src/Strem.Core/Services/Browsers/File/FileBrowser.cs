using NativeFileDialogSharp;

namespace Strem.Core.Services.Browsers.File;

public class FileBrowser : IFileBrowser
{
    public string BrowseForFile(string startingDirectory = null)
    {
        var result = Dialog.FileOpen(null, startingDirectory);
        return !result.IsOk ? string.Empty : result.Path;
    }
}