using Strem.Flows.Data.Tasks;

namespace Strem.Flows.Data;

public interface IHasSubTaskData
{
    string[] SubTaskKeys { get; }
    Dictionary<string, List<IFlowTaskData>> SubTasks { get; }
}