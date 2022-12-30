namespace Strem.Core.Plugins;

public interface IPluginRegistry
{
    IReadOnlyCollection<PluginInfo> GetPluginInfo();
}