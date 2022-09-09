using LiteDB;
using Strem.Core.Flows;
using Strem.Data;
using Strem.Data.Types;
using Strem.Todos.Data;
using Strem.Todos.Data.Repositories;

namespace Strem.Portals.Data.Repositories;

public class TodoRepository : Repository<TodoData, Guid>, ITodoRepository
{
    public TodoRepository(ILiteDatabase connection) : base(connection, "todos")
    {}

    public override BsonValue GetId(Guid id) => new(id);
}