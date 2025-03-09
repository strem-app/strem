using Cake.Core;
using Cake.Frosting;

namespace Strem.Build;

public class BuildContext : FrostingContext
{
    public string Version { get; set; }
    public string Platform { get; set; }
    
    public BuildContext(ICakeContext context) : base(context)
    {
        Version = context.Arguments.GetArgument("buildNo") ?? "0.0.0";
        Platform = context.Arguments.GetArgument("platform") ?? "win-x64";
    }
}