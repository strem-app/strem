using Cake.Frosting;
using Strem.Build.Tasks;

namespace Strem.Build.Orchestration;

[TaskName("publish")]
[IsDependentOn(typeof(PackageAppTask))]
[IsDependentOn(typeof(PackageLibsTask))]
public class PublishTask : FrostingTask<BuildContext>
{
    
}