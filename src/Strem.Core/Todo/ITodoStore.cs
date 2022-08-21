namespace Strem.Core.Todo;

public interface ITodoStore
{
    List<TodoData> Todos { get; }
}