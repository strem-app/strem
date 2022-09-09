using Strem.Data;

namespace Strem.Todos.Data.Repositories;

public interface ITodoRepository : IRepository<TodoData, Guid>
{}