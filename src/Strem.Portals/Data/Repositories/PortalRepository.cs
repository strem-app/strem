using LiteDB;
using Strem.Core.Flows;
using Strem.Data;
using Strem.Data.Types;

namespace Strem.Portals.Data.Repositories;

public class PortalRepository : Repository<PortalData, Guid>, IPortalRepository
{
    public PortalRepository(ILiteDatabase connection) : base(connection, "portals")
    {}

    public override BsonValue GetId(Guid id) => new(id);
}