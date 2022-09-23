using Strem.Core.State;
using Strem.OBS.Variables;

namespace Strem.OBS.Extensions;

public static class IAppSettingsExtensions
{
    public static bool HasOBSHost(this IAppState state) => state.AppVariables.Has(OBSVars.Host);
    
    public static string GetCurrentSceneName(this IAppState state) => state.TransientVariables.Get(OBSVars.CurrentScene);
    public static void SetCurrentSceneName(this IAppState state, string sceneName) => state.TransientVariables.Set(OBSVars.CurrentScene, sceneName);
}