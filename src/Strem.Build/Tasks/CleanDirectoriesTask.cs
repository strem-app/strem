using Cake.Frosting;
using Strem.Build.Extensions;

namespace Strem.Build.Tasks;

public class CleanDirectoriesTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    { context.CreateOrCleanDirectory(Directories.Dist); }
}