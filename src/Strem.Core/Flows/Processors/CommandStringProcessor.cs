using System.Text.RegularExpressions;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;

namespace Strem.Core.Flows.Processors;

public class CommandStringProcessor : ICommandStringProcessor
{
    public readonly Regex CommandPattern = new(@"^\!([\w\._\-\:]*)\s?(.*)?");
    
    public ParsedCommand Process(string textToProcess)
    {
        var matches = CommandPattern.Match(textToProcess);
        if(matches.Groups.Count < 2)
        { return new ParsedCommand(); }

        var commandName = matches.Groups[1].Value;
        if(matches.Groups.Count == 2)
        { return new ParsedCommand(commandName); }

        var commandArgs = matches.Groups[2].Value;
        return new ParsedCommand(commandName, commandArgs);
    }
}