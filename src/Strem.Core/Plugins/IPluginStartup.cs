namespace Strem.Core.Plugins;

public interface IPluginStartup : IDisposable
{
    /// <summary>
    /// This indicates the plugin REQUIRES these configuration keys to run
    /// </summary>
    string[] RequiredConfigurationKeys => Array.Empty<string>();
    
    /// <summary>
    /// Any processing which needs to be done before the plugin is started and keys checked
    /// </summary>
    /// <remarks>
    /// This method is executed before keys are checked allowing dynamic plugins to register any required keys or to
    /// wait for other plugins to register runtime app config keys for other plugins to depend upon
    /// </remarks>
    /// <returns>A task indicating completion state</returns>
    Task SetupPlugin();
    
    /// <summary>
    /// Any processing which needs to be done to start the plugin
    /// </summary>
    /// <returns></returns>
    Task StartPlugin();
}