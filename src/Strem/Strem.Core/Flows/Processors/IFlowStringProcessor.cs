using Strem.Core.Variables;

namespace Strem.Core.Flows.Processors;

public interface IFlowStringProcessor
{
    string Process(string textToProcess, IVariables flowVariables);
}