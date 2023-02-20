using Strem.Core.Types;
using Strem.Flows.Data.Triggers;

namespace Strem.StreamElements.Flows.Triggers;

public class OnStreamElementsRedemptionTriggerData : IFlowTriggerData
{
    public static readonly string TriggerCode = "on-stream-elements-redemption";
    public static readonly string TriggerVersion = "1.0.0";

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TriggerCode;
    public string Version { get; set; } = TriggerVersion;

    public string StoreItemName { get; set; } = string.Empty;
    public TextMatchType NameMatchType { get; set; } = TextMatchType.Match;
}