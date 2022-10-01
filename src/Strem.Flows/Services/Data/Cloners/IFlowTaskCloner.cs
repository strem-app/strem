using Strem.Flows.Data.Tasks;

namespace Strem.Flows.Services.Data.Cloners;

public interface IFlowTaskCloner
{
    public IFlowTaskData Clone(IFlowTaskData taskToClone);
}