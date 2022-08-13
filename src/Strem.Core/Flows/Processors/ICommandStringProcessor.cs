namespace Strem.Core.Flows.Processors;

public interface ICommandStringProcessor
{
    ParsedCommand Process(string textToProcess);
}