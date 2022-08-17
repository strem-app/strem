namespace Strem.Core.Flows;

public class FlowStore : IFlowStore
{
    public const string DefaultFlowCategory = "Default";

    public List<Flow> Flows { get; set; } = new();
}