using Strem.Core.Types;

namespace Strem.Core.Plugins;

public record PluginInfo(string Name, string Version, PluginLoadState LoadState);