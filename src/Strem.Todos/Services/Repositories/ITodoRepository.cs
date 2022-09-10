using Strem.Data;
using Strem.Todos.Data;

namespace Strem.Todos.Services.Repositories;

public interface ITodoRepository : IRepository<TodoData, Guid>
{}