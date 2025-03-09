using Cake.Frosting;
using Strem.Build.Tasks;

namespace Strem.Build.Orchestration;

[TaskName("publish-app")]
[IsDependentOn(typeof(PackageAppTask))]
public class PublishAppTask : FrostingTask<BuildContext>
{
    
}