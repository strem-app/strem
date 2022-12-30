using Strem.Infrastructure.Services.Api;

namespace Strem.Infrastructure.Services;

public class StremPathHelper
{
    public static string StremDataDirectory =  $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}Strem{Path.DirectorySeparatorChar}";
    public static string BackupIndicatorFile =  $"{StremDataDirectory}{Path.DirectorySeparatorChar}._backup";
    
    public static string PluginPath = Path.GetFullPath($"{Environment.CurrentDirectory}/Plugins/");
    public static string PluginWebDirectory = $"http://localhost:{InternalWebHostConfiguration.ApiHostPort}/Plugins";
    public static string GetPluginRootPath(string pluginName) => $"{PluginWebDirectory}/{pluginName}";
}