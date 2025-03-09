using Cake.Frosting;
using Strem.Build.Tasks;

namespace Strem.Build.Orchestration;

[TaskName("publish-libs")]
[IsDependentOn(typeof(PackageLibsTask))]
public class PublishTask : FrostingTask<BuildContext>
{
    
}