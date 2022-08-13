using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Frosting;
using GlobExpressions;

namespace Strem.Build.Tasks;

public class BuildSolutionTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var buildSettings = new DotNetBuildSettings
        {
            Configuration = "Release",
        };

        context.DotNetBuild($"{Directories.Src}/Strem.sln", buildSettings);
    }
}