using Strem.Core.Variables;

namespace Strem.Flows.Processors;

public interface IFlowStringProcessor
{
    string Process(string textToProcess, IVariables flowVariables);
}