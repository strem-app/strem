using NativeFileDialogSharp;

namespace Strem.Core.Services.Browsers.File;

public class FileBrowser : IFileBrowser
{
    public string BrowseToOpenFile(string startingDirectory = null, string filterList = null)
    {
        var result = Dialog.FileOpen(filterList, startingDirectory);
        return !result.IsOk ? string.Empty : result.Path;
    }
    
    public string BrowseToSaveFile(string startingDirectory = null, string filterList = null)
    {
        var result = Dialog.FileSave(filterList, startingDirectory);
        return !result.IsOk ? string.Empty : result.Path;
    }
}