using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Frosting;

namespace Strem.Build.Tasks;

public class PackageLibsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var nuGetPackSettings = new DotNetPackSettings
        {
            OutputDirectory = Directories.Dist,
            MSBuildSettings = new DotNetMSBuildSettings() { Version = context.Version }
        };
        
        context.DotNetPack($"./{Directories.Src}/Strem.sln", nuGetPackSettings);
    }
}