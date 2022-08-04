using Strem.Core.Flows.Tasks;
using Strem.Core.Types;

namespace Strem.Flows.Default.Flows.Tasks.Data;

public class SetVariableTaskData : IFlowTaskData
{
    public string Code => SetVariableTask.TaskCode;
    public string Version { get; set; } = SetVariableTask.TaskVersion;
    
    public string Name { get; set; }
    public string Context { get; set; }
    public string Value { get; set; }
    public VariableScope Scope { get; set; }
}