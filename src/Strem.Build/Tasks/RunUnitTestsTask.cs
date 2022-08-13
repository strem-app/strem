using Cake.Frosting;
using Strem.Build.Extensions;

namespace Strem.Build.Tasks;

public class RunUnitTestsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetTestAndLog("UnitTests");
    }
}