using Strem.Core.State;
using Strem.OBS.Variables;

namespace Strem.OBS.Extensions;

public static class IAppSettingsExtensions
{
    public static bool HasOBSHost(this IAppState state) => state.AppVariables.Has(OBSVars.Host);
}