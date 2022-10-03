using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Frosting;

namespace Strem.Build.Tasks;

[IsDependentOn(typeof(CleanDirectoriesTask))]
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
        MoveStaticIntoContent(context, outputDirectory);
        context.Zip(outputDirectory, $"{outputDirectory}.zip");
    }

    public void MoveStaticIntoContent(BuildContext context, string outputDirectory)
    {
        // TODO: This is a bodge as for some reason it doesnt do this automatically
        var wwwroot = $"{outputDirectory}/wwwroot";
        var stremContent = $"{wwwroot}/_content/Strem";
        context.CreateDirectory(stremContent);
        context.CopyFiles($"{wwwroot}/*", $"{stremContent}");
        context.CopyDirectory($"{wwwroot}/css",$"{stremContent}/css");
        context.CopyDirectory($"{wwwroot}/js",$"{stremContent}/js");
        context.CopyDirectory($"{wwwroot}/webfonts",$"{stremContent}/webfonts");
    }
}