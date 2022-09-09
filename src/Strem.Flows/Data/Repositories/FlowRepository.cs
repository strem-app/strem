using LiteDB;
using Strem.Data;

namespace Strem.Flows.Data.Repositories;

public class FlowRepository : Repository<Flow, Guid>, IFlowRepository
{
    public FlowRepository(ILiteDatabase connection) : base(connection, "flows")
    {}

    public override BsonValue GetId(Guid id) => new(id);
}