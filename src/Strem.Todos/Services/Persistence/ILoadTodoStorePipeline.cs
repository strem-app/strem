using Strem.Infrastructure.Services.Persistence.Generic;
using Strem.Todos.Data;

namespace Strem.Todos.Services.Persistence;

public interface ILoadTodoStorePipeline : ILoadDataPipeline<TodoStore>
{
}