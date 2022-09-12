using Strem.Flows.Data;

namespace Strem.Flows.Services.Data;

public record FlowDataWrapper(IReadOnlyCollection<Flow> Flows);