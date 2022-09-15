using Strem.Core.Services.Registries;

namespace Strem.Flows.Services.Registries.Tasks;

public class TaskRegistry : Registry<TaskDescriptor>, ITaskRegistry
{
    public override string GetId(TaskDescriptor data) => data.Task.Code;
}