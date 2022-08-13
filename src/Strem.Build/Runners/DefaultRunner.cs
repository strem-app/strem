using Cake.Frosting;
using Strem.Build.Tasks;

namespace Strem.Build.Runners;

[TaskName("default")]
[IsDependentOn(typeof(PackageAppTask))]
public class DefaultRunner : FrostingTask<BuildContext>
{
    
}