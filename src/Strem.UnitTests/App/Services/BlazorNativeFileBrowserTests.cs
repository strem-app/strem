using Strem.Services.Dialogs;

namespace Strem.UnitTests.App.Services;

public class BlazorNativeFileBrowserTests
{
    [Fact]
    public void should_correctly_convert_filter_string_to_filter_tuple()
    {
        var filterString = "Excel Files (*.xls, *.xlsx)|*.xls;*.xlsx|CSV Files (*.csv)|*.csv";
        var transformedFilters = BlazorNativeFileBrowser.FilterToBlazorFilter(filterString);
        Assert.Equal(2, transformedFilters.Length);
        Assert.Equal("Excel Files (*.xls, *.xlsx)", transformedFilters[0].Name);
        Assert.Equal(2, transformedFilters[0].Extensions.Length);
        Assert.Contains("*.xls", transformedFilters[0].Extensions);
        Assert.Contains("*.xlsx", transformedFilters[0].Extensions);
        Assert.Equal("CSV Files (*.csv)", transformedFilters[1].Name);
        Assert.Equal(1, transformedFilters[1].Extensions.Length);
        Assert.Contains("*.csv", transformedFilters[1].Extensions);
    }
}