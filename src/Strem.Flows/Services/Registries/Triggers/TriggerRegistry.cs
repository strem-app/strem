using Strem.Core.Services.Registries;

namespace Strem.Flows.Services.Registries.Triggers;

public class TriggerRegistry : Registry<TriggerDescriptor>, ITriggerRegistry
{
    public override string GetId(TriggerDescriptor data) => data.Trigger.Code;
}