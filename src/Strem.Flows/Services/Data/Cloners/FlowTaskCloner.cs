using Strem.Core.Extensions;
using Strem.Core.Services.Utils;
using Strem.Flows.Data;
using Strem.Flows.Data.Tasks;

namespace Strem.Flows.Services.Data.Cloners;

public class FlowTaskCloner : IFlowTaskCloner
{
    public ICloner Cloner { get; }

    public FlowTaskCloner(ICloner cloner)
    {
        Cloner = cloner;
    }
    
    public IFlowTaskData Clone(IFlowTaskData taskToClone)
    {
        var clonedTask = Cloner.Clone(taskToClone);
        clonedTask.Id = Guid.NewGuid();
        
        if (clonedTask is IHasSubTaskData subTaskData)
        { subTaskData.SubTasks.Values.ForEach(RewriteIdsForTasks); }

        return clonedTask;
    }
    
    public void RewriteIdsForTasks(IReadOnlyCollection<IFlowTaskData> tasks)
    {
        foreach (var task in tasks)
        {
            task.Id = Guid.NewGuid();
            if (task is IHasSubTaskData subTaskData)
            { subTaskData.SubTasks.Values.ForEach(RewriteIdsForTasks); }
        }
    }
}