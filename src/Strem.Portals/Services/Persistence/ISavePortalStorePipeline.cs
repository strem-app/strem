using Strem.Infrastructure.Services.Persistence.Generic;
using Strem.Portals.Data;

namespace Strem.Portals.Services.Persistence;

public interface ISavePortalStorePipeline : ISaveDataPipeline<IPortalStore>
{
}