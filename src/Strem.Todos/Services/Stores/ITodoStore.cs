using Strem.Data.Stores;
using Strem.Todos.Data;

namespace Strem.Todos.Services.Stores;

public interface ITodoStore : IInMemoryStore<TodoData, Guid>
{
}