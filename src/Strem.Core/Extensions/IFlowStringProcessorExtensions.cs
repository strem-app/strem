using Strem.Core.Flows.Processors;
using Strem.Core.Variables;

namespace Strem.Core.Extensions;

public static class IFlowStringProcessorExtensions
{
    public static bool TryProcessInt(this IFlowStringProcessor processor, string inValue, IVariables flowVariables, out int outValue)
    {
        var processedWaitAmount = processor.Process(inValue, flowVariables);
        return int.TryParse(processedWaitAmount, out outValue);
    }
    
    public static bool TryProcessFloat(this IFlowStringProcessor processor, string inValue, IVariables flowVariables, out float outValue)
    {
        var processedWaitAmount = processor.Process(inValue, flowVariables);
        return float.TryParse(processedWaitAmount, out outValue);
    }
}