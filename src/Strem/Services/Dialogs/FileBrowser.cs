using Photino.Blazor;
using Strem.Core.Services.Browsers.File;

namespace Strem.Services.Dialogs;

public class FileBrowser : IFileBrowser
{
    public PhotinoBlazorApp App { get; }

    public FileBrowser(PhotinoBlazorApp app)
    {
        App = app;
    }

    // EXAMPLE "Excel Files (*.xls, *.xlsx)|*.xls;*.xlsx|CSV Files (*.csv)|*.csv"
    public static (string Name, string[] Extensions)[] FilterToBlazorFilter(string filter)
    {
        if (string.IsNullOrEmpty(filter))
        { return Array.Empty<(string Name, string[] Extensions)>(); }

        var filterList = new List<(string Name, string[] Extensions)>();
        var lineFilters = filter.Split(";");
        foreach (var lineFilter in lineFilters)
        {
            if (!lineFilter.Contains('|'))
            { filterList.Add((lineFilter, new[] { lineFilter })); }
            else
            {
                var filerSections = lineFilter.Split("|");
                filterList.Add((filerSections[0], filerSections[1].Split(",")));
            }
        }
    }

    public string BrowseToOpenFile(string startingDirectory = null, string filterList = null)
    {
        var filters = 
        App.MainWindow.ShowOpenFile(defaultPath: startingDirectory, filters: filters);
        var result = Dialog.FileOpen(filterList, startingDirectory);
        return !result.IsOk ? string.Empty : result.Path;
    }
    
    public string BrowseToSaveFile(string startingDirectory = null, string filterList = null)
    {
        var result = Dialog.FileSave(filterList, startingDirectory);
        return !result.IsOk ? string.Empty : result.Path;
    }
}