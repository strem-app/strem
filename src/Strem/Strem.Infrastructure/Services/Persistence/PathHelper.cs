namespace Strem.Infrastructure.Services.Persistence;

public class PathHelper
{
    public static string AppDirectory =  $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}Strem{Path.DirectorySeparatorChar}";
}