namespace Strem.Infrastructure.Services.Persistence;

public class PathHelper
{
    public static string StremDataDirectory =  $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}Strem{Path.DirectorySeparatorChar}";
}