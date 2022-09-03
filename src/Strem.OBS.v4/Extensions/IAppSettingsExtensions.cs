using Strem.Core.State;
using Strem.OBS.v4.Variables;

namespace Strem.OBS.v4.Extensions;

public static class IAppSettingsExtensions
{
    public static bool HasOBSHost(this IAppState state) => state.AppVariables.Has(OBSVars.Host);
    
    public static string GetCurrentSceneName(this IAppState state) => state.TransientVariables.Get(OBSVars.CurrentScene);
    public static void SetCurrentSceneName(this IAppState state, string sceneName) => state.TransientVariables.Set(OBSVars.CurrentScene, sceneName);
    public static bool HasSourceList(this IAppState state) => state.TransientVariables.Has(OBSVars.SourceItems);
    public static bool HasSourceItem(this IAppState state, string item) => state.TransientVariables.Get(OBSVars.SourceItems).Contains(item, StringComparison.OrdinalIgnoreCase);
    public static string[] GetSourceList(this IAppState state) => state.TransientVariables.Get(OBSVars.SourceItems).Split(",");
}