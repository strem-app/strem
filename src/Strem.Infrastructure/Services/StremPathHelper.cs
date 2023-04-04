using Strem.Infrastructure.Services.Api;

namespace Strem.Infrastructure.Services;

public class StremPathHelper
{
    /// <summary>
    /// The applications root path, this is where the exe lives
    /// </summary>
    public static string AppPath = Path.GetDirectoryName(Environment.ProcessPath) ?? Environment.CurrentDirectory;

    /// <summary>
    /// This is where all logs should be output to, based off the app path
    /// </summary>
    public static string LogPath = Path.GetFullPath($"{AppPath}/Logs/");
    
    /// <summary>
    /// This is where the strem user data should be stored
    /// </summary>
    public static string StremDataDirectory =  $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}Strem{Path.DirectorySeparatorChar}";
    
    /// <summary>
    /// This is where the indicator that a backup is needed should live
    /// </summary>
    public static string BackupIndicatorFile =  $"{StremDataDirectory}{Path.DirectorySeparatorChar}._backup";

    /// <summary>
    /// This is where the plugins should reside, based off the app path
    /// </summary>
    public static string PluginPath = Path.GetFullPath($"{AppPath}/Plugins/");
    
    /// <summary>
    /// This is the url for checking plugins in the web directory
    /// </summary>
    public static string PluginWebDirectory = $"http://localhost:{InternalWebHostConfiguration.ApiHostPort}/Plugins";
    
    /// <summary>
    /// Helper for getting paths to a given plugin name
    /// </summary>
    /// <param name="pluginName">The name of the plugin to lookup</param>
    /// <returns>The url to the plugins path</returns>
    public static string GetPluginRootPath(string pluginName) => $"{PluginWebDirectory}/{pluginName}";
}