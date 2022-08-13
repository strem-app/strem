namespace Strem.Core.Flows.Processors;

public class ParsedCommand
{
    public string CommandName { get; }
    public string CommandArgs { get; }

    public ParsedCommand(string commandName = "", string commandArgs = "")
    {
        CommandName = commandName;
        CommandArgs = commandArgs;
    }
}