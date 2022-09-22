namespace Strem.Infrastructure.Services;

public class StremPathHelper
{
    public static string StremDataDirectory =  $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}Strem{Path.DirectorySeparatorChar}";
    public static string BackupIndicatorFile =  $"{StremDataDirectory}{Path.DirectorySeparatorChar}._backup";
}