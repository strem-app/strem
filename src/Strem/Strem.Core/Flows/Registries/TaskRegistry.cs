using Strem.Core.Flows.Tasks;

namespace Strem.Core.Flows.Registries;

public class TaskRegistry : ITaskRegistry
{
    public List<IFlowTask> Tasks { get; }

    public TaskRegistry(IEnumerable<IFlowTask> tasks = null)
    {
        Tasks = tasks?.ToList() ?? new List<IFlowTask>();
    }

    public void Add(IFlowTask task) => Tasks.Add(task);
    public void Remove(IFlowTask task) => Tasks.Remove(task);
    public IFlowTask Get(string taskCode) => Tasks.SingleOrDefault(x => x.Code == taskCode);
    public IEnumerable<IFlowTask> GetAll() => Tasks;
}