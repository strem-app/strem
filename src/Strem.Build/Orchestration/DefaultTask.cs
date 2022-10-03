using Cake.Frosting;
using Strem.Build.Tasks;

namespace Strem.Build.Orchestration;

[TaskName("default")]
[IsDependentOn(typeof(CleanDirectoriesTask))]
[IsDependentOn(typeof(PackageAppTask))]
[IsDependentOn(typeof(PackageLibsTask))]
public class DefaultTask : FrostingTask<BuildContext>
{
    
}