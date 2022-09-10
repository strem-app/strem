using LiteDB;
using Strem.Data;
using Strem.Flows.Data;

namespace Strem.Flows.Services.Repositories;

public class FlowRepository : Repository<Flow, Guid>, IFlowRepository
{
    public FlowRepository(ILiteDatabase connection) : base(connection, "flows")
    {}

    public override BsonValue GetId(Guid id) => new(id);
}