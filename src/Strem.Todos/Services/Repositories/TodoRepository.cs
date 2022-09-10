using LiteDB;
using Strem.Data;
using Strem.Todos.Data;

namespace Strem.Todos.Services.Repositories;

public class TodoRepository : Repository<TodoData, Guid>, ITodoRepository
{
    public TodoRepository(ILiteDatabase connection) : base(connection, "todos")
    {}

    public override BsonValue GetId(Guid id) => new(id);
}