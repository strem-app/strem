using Cake.Frosting;
using Strem.Build.Tasks;

namespace Strem.Build.Orchestration;

[TaskName("default")]
[IsDependentOn(typeof(PackageAppTask))]
public class DefaultTask : FrostingTask<BuildContext>
{
    
}