namespace Strem.Core.Flows;

public class FlowStore : IFlowStore
{
    public const string DefaultFlowStore = "Default";

    public List<Flow> Flows { get; set; } = new();
}