using LiteDB;
using Strem.Core.Flows;

namespace Strem.Data.Types;

public class FlowRepository : Repository<Flow, Guid>, IFlowRepository
{
    public FlowRepository(ILiteDatabase connection) : base(connection, "flows")
    {}

    public override BsonValue GetId(Guid id) => new(id);
}