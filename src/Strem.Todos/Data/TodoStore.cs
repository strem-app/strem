namespace Strem.Todos.Data;

public class TodoStore : ITodoStore
{
    public List<TodoData> Todos { get; set; } = new();

    public TodoStore(IEnumerable<TodoData>? todoData = null)
    {
        if(todoData != null)
        { Todos.AddRange(todoData); }
    }
}