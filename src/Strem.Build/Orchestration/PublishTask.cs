using Cake.Frosting;
using Strem.Build.Tasks;

namespace Strem.Build.Orchestration;

[TaskName("publish")]
[IsDependentOn(typeof(PackageAppTask))]
public class PublishTask : FrostingTask<BuildContext>
{
    
}