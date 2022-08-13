using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Frosting;

namespace Strem.Build.Tasks;

[IsDependentOn(typeof(CleanDirectoriesTask))]
[IsDependentOn(typeof(BuildSolutionTask))]
[IsDependentOn(typeof(RunUnitTestsTask))]
public class PackageAppTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var outputDirectory = $"{Directories.Dist}/Strem-{context.Version}";
        if(!Directory.Exists(outputDirectory))
        { context.CreateDirectory(outputDirectory); }

        var appProject = $"{Directories.Src}/Strem/Strem.csproj";
        var publishSettings = new DotNetPublishSettings
        {
            Configuration = "Release",
            Runtime = "win-x64",
            OutputDirectory = outputDirectory,
            SelfContained = true,
            MSBuildSettings = new DotNetMSBuildSettings
            {
                Version = context.Version
            }
        };
        context.DotNetPublish(appProject, publishSettings);
        context.Zip(outputDirectory, $"{outputDirectory}.zip");
    }
}