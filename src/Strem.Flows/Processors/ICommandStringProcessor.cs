namespace Strem.Flows.Processors;

public interface ICommandStringProcessor
{
    ParsedCommand Process(string textToProcess);
}