namespace Strem.Infrastructure.Services;

public class PathHelper
{
    public static string StremDataDirectory =  $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}Strem{Path.DirectorySeparatorChar}";
}