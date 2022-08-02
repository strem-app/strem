using Strem.Core.Flows.Tasks;

namespace Strem.Core.Flows.Registries;

public class TaskRegistry : ITaskRegistry
{
    public List<TaskDescriptor> Tasks { get; }

    public TaskRegistry(IEnumerable<TaskDescriptor> tasks = null)
    {
        Tasks = tasks?.ToList() ?? new List<TaskDescriptor>();
    }

    public void Add(TaskDescriptor task) => Tasks.Add(task);
    public void Remove(TaskDescriptor task) => Tasks.Remove(task);
    public TaskDescriptor Get(string taskCode) => Tasks.SingleOrDefault(x => x.Task.Code == taskCode);
    public IEnumerable<TaskDescriptor> GetAll() => Tasks;
}