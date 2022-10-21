namespace Strem.Core.Plugins;

/// <summary>
/// Represents a collection of dependencies to register within the DI container (ServiceCollection)
/// </summary>
public interface IDependencyModule
{
    /// <summary>
    /// This is the initial service setup area
    /// </summary>
    /// <param name="services">The services object you wish to setup dependencies on</param>
    void Setup(IServiceCollection services);

    /// <summary>
    /// This indicates the plugin REQUIRES these configuration keys to run
    /// </summary>
    string[] RequiredConfigurationKeys => Array.Empty<string>();
}