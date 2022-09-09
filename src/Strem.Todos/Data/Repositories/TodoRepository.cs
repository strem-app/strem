using LiteDB;
using Strem.Data;

namespace Strem.Todos.Data.Repositories;

public class TodoRepository : Repository<TodoData, Guid>, ITodoRepository
{
    public TodoRepository(ILiteDatabase connection) : base(connection, "todos")
    {}

    public override BsonValue GetId(Guid id) => new(id);
}