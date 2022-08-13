using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Coverlet;
using GlobExpressions;
using Path = System.IO.Path;

namespace Strem.Build.Extensions;

public class DotNetTestAndLogSettings
{
    /// <summary>
    /// The test type is used in a regex expression to find all projects containing that string.
    /// </summary>
    public string? TestTypes { get; set; }
        
    /// <summary>
    /// The directory to output logs to
    /// </summary>
    public string OutputDir { get; set; }

    /// <summary>
    /// Should include code coverage too
    /// </summary>
    public bool IncludeCodeCoverage { get; set; } = true;
        
    /// <summary>
    /// The types of loggers to use
    /// </summary>
    public string[] LogTypes { get; set; } = {"html", "trx"};
        
    /// <summary>
    /// Any environmental properties to pass onto the test runner
    /// </summary>
    public Dictionary<string, string> EnvironmentalVars { get; set; }
}

public static class BuildContextTestingExtensions
{
    /// <summary>
    /// Runs given tests and outputs logs to output dir
    /// </summary>
    /// <param name="context">The context to run on</param>
    /// <param name="testTypes">The name to match in the regex of project names</param>
    public static void DotNetTestAndLog(this BuildContext context, string testTypes, bool includeCoverage = true)
    {
        var settings = new DotNetTestAndLogSettings
        {
            TestTypes = testTypes, 
            IncludeCodeCoverage = includeCoverage
        };
        DotNetTestAndLog(context, settings);
    }

    /// <summary>
    /// Runs given tests and outputs logs to output dir
    /// </summary>
    /// <param name="context">The context to run on</param>
    /// <param name="settings">The settings to apply</param>
    public static void DotNetTestAndLog(this BuildContext context, DotNetTestAndLogSettings settings)
    {
        var testProjects = Glob.Files(Directories.Src, $"**/*{settings.TestTypes}*.csproj");
        foreach (var testProject in testProjects)
        {
            var actualProjLocation = $"{Directories.Src}/{testProject}";
            context.Log.Information($"Found Test Project: {actualProjLocation}");
            var logFileName = Path.GetFileNameWithoutExtension(testProject);
            var loggers = settings.LogTypes
                    .Select(x => $"{x};LogFileName={logFileName}.Test.Results.{x}")
                    .ToArray();

            var actualOutputDir = settings.OutputDir ?? Directories.Dist;
            var testSettings = new DotNetCoreTestSettings
            {
                ResultsDirectory = new DirectoryPath(actualOutputDir),
                Loggers = loggers,
                EnvironmentVariables = settings.EnvironmentalVars
            };

            if (settings.IncludeCodeCoverage)
            {
                var coverletSettings = new CoverletSettings {
                    CollectCoverage = settings.IncludeCodeCoverage,
                    CoverletOutputFormat = CoverletOutputFormat.opencover,
                    CoverletOutputDirectory = actualOutputDir,
                    CoverletOutputName = $"{logFileName}.Cover.Results.xml"
                };
            
                context.DotNetCoreTest(actualProjLocation, testSettings, coverletSettings);
            }
            else
            { context.DotNetTest(actualProjLocation, testSettings); }
        }
    }
}