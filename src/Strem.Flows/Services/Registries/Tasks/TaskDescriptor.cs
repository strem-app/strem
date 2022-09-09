using Strem.Flows.Data.Tasks;

namespace Strem.Flows.Services.Registries.Tasks;

public class TaskDescriptor
{
    public IFlowTask Task { get; }
    public Func<IFlowTaskData> Data { get; }
    public Type ComponentType { get; }

    public TaskDescriptor(IFlowTask task, Func<IFlowTaskData> data, Type componentType)
    {
        Task = task;
        Data = data;
        ComponentType = componentType;
    }
}