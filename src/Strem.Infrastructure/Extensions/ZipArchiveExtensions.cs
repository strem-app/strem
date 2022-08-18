using System.IO.Compression;
using GlobExpressions;

namespace Strem.Infrastructure.Extensions;

public static class ZipArchiveExtensions
{
    public static void CreateEntryFromGlob(this ZipArchive archive, string directory, string pattern)
    {
        var fileGlob = Glob.Files(directory, pattern).ToArray();
        foreach (var file in fileGlob)
        {
            archive.CreateEntryFromFile($"{directory}/{file}", Path.GetFileName(file));
        }
    }
}