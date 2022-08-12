namespace Strem.Core.Flows.Registries.Tasks;

public class TaskRegistry : ITaskRegistry
{
    public Dictionary<string, TaskDescriptor> Tasks { get; } = new();
    public void Add(TaskDescriptor task) => Tasks.Add(task.Task.Code, task);
    public void Remove(TaskDescriptor task) => Tasks.Remove(task.Task.Code);
    public TaskDescriptor Get(string taskCode) => Tasks.ContainsKey(taskCode) ? Tasks[taskCode] : null;
    public IEnumerable<TaskDescriptor> GetAll() => Tasks.Values;
}