namespace Strem.Todos.Data;

public interface ITodoStore
{
    List<TodoData> Todos { get; }
}