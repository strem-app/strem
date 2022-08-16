using Cake.Frosting;
using Strem.Build.Tasks;

namespace Strem.Build.Orchestration;

[TaskName("build-and-test")]
[IsDependentOn(typeof(CleanDirectoriesTask))]
[IsDependentOn(typeof(BuildSolutionTask))]
[IsDependentOn(typeof(RunUnitTestsTask))]
public class BuildAndTestTask : FrostingTask<BuildContext>
{
    
}