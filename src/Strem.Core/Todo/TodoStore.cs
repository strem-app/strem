namespace Strem.Core.Todo;

public class TodoStore : ITodoStore
{
    public const string DefaultFlowStore = "Default";

    public List<TodoData> Todos { get; set; } = new();
}