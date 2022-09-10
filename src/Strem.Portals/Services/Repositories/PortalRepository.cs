using LiteDB;
using Strem.Data;
using Strem.Portals.Data;

namespace Strem.Portals.Services.Repositories;

public class PortalRepository : Repository<PortalData, Guid>, IPortalRepository
{
    public PortalRepository(ILiteDatabase connection) : base(connection, "portals")
    {}

    public override BsonValue GetId(Guid id) => new(id);
}