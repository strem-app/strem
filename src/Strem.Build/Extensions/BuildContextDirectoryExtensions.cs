using Cake.Common.IO;

namespace Strem.Build.Extensions;

public static class BuildContextDirectoryExtensions
{
    public static void CreateOrCleanDirectory(this BuildContext context, string directory)
    {
        if(context.DirectoryExists(directory))
        { context.CleanDirectory(directory); }
        else
        { context.CreateDirectory(directory);}
    }
}