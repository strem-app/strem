using Strem.Flows.Data;

namespace Strem.Flows.Services.Data.Cloners;

public interface IFlowCloner
{
    public Flow Clone(Flow flowToClone);
}