using Cake.Core;
using Cake.Frosting;

namespace Strem.Build;

public class BuildContext : FrostingContext
{
    public BuildContext(ICakeContext context) : base(context)
    {
    }
}