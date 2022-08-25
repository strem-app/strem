using Strem.Core.Types;
using Strem.Core.Variables;

namespace Strem.Core.Flows.Executors;

public class FlowExecutionLog
{
    public Guid FlowId { get; set; }
    public string FlowName { get; set; }
    public ExecutionResultType ExecutionResultType { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public IVariables StartVariables { get; set; }
    public IVariables EndVariables { get; set; }
    public List<string> ElementExecutionSummary { get; set; } = new();
}