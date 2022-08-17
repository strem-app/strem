using Strem.Core.Todo;

namespace Strem.Core.Flows;

public interface ITodoStore
{
    List<TodoData> Todos { get; }
}