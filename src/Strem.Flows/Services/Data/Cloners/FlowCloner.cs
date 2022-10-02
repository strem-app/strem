using Strem.Core.Extensions;
using Strem.Core.Services.Utils;
using Strem.Flows.Data;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Data.Triggers;

namespace Strem.Flows.Services.Data.Cloners;

public class FlowCloner : IFlowCloner
{
    public ICloner Cloner { get; }

    public FlowCloner(ICloner cloner)
    {
        Cloner = cloner;
    }

    public Flow Clone(Flow flowToClone)
    {
        var clonedFlow = Cloner.Clone(flowToClone);
        clonedFlow.Id = Guid.NewGuid();
        clonedFlow.Name = $"Copy Of {clonedFlow.Name}";
        clonedFlow.Enabled = false;

        RewriteIdsForTriggers(clonedFlow.TriggerData);
        RewriteIdsForTasks(clonedFlow.TaskData);
        
        return clonedFlow;
    }

    public void RewriteIdsForTriggers(IReadOnlyCollection<IFlowTriggerData> triggers)
    { triggers.ForEach(x => x.Id = Guid.NewGuid()); }

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