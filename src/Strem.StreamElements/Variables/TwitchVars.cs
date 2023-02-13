using Strem.Core.Variables;

namespace Strem.StreamElements.Variables;

public class StreamElementsVars
{
    // Generic
    public static readonly string StreamElementsContext = "stream-elements";
    
    // OAuth (app)
    public static readonly VariableEntry JwtToken = new("jwt-token", StreamElementsContext);
}