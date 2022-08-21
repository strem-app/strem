using Strem.Core.Portals;
using Strem.Infrastructure.Services.Persistence.Generic;

namespace Strem.Infrastructure.Services.Persistence.Portals;

public interface ISavePortalStorePipeline : ISaveDataPipeline<IPortalStore>
{
}