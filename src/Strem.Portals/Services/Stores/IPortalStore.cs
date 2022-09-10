using Strem.Data.Stores;
using Strem.Portals.Data;

namespace Strem.Portals.Services.Stores;

public interface IPortalStore : IInMemoryStore<PortalData, Guid>
{ }