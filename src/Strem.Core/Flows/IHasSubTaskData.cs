using Strem.Core.Flows.Tasks;

namespace Strem.Core.Flows;

public interface IHasSubTaskData
{
    Dictionary<string, List<IFlowTaskData>> SubTasks { get; }
}