using Strem.Core.State;
using Strem.OBS.Variables;

namespace Strem.OBS.Extensions;

public static class IAppSettingsExtensions
{
    public static bool HasOBSHost(this IAppState state) => state.AppVariables.Has(OBSVars.Host);
    
    public static string GetCurrentSceneName(this IAppState state) => state.TransientVariables.Get(OBSVars.CurrentScene);
    public static bool HasCurrentSceneItems(this IAppState state) => state.TransientVariables.Has(OBSVars.CurrentSceneItems);
    public static bool HasCurrentSceneItem(this IAppState state, string item) => state.TransientVariables.Get(OBSVars.CurrentSceneItems).Contains(item, StringComparison.OrdinalIgnoreCase);
    public static string[] GetCurrentSceneItems(this IAppState state) => state.TransientVariables.Get(OBSVars.CurrentSceneItems).Split(",");
}