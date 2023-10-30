using Strem.Core.State;
using Strem.StreamElements.Variables;

namespace Strem.StreamElements.Extensions;

public static class IAppStateExtensions
{
    public static bool HasStreamElementsJwtToken(this IAppState state) => state.AppVariables.Has(StreamElementsVars.JwtToken);
}