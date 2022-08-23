using Strem.Core.Flows.Tasks;

namespace Strem.Core.Flows;

public interface IHasSubTaskData
{
    string[] SubTaskKeys { get; }
    Dictionary<string, List<IFlowTaskData>> SubTasks { get; }
}