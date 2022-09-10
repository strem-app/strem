using Strem.Data.Stores;
using Strem.Flows.Data;

namespace Strem.Flows.Services.Stores;

public interface IFlowStore : IInMemoryStore<Flow, Guid>
{ }