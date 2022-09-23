namespace Strem.Core.Plugins;

public interface IPluginStartup
{
    /// <summary>
    /// This indicates the plugin REQUIRES these configuration keys to run
    /// </summary>
    string[] RequiredConfigurationKeys { get; }
    
    /// <summary>
    /// Any processing which needs to be done before the plugin is started and keys checked
    /// </summary>
    /// <returns>A task indicating completion state</returns>
    Task SetupPlugin();
    
    /// <summary>
    /// Any processing which needs to be done to start the plugin
    /// </summary>
    /// <returns></returns>
    Task StartPlugin();
}