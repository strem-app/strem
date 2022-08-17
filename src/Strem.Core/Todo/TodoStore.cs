using Strem.Core.Todo;

namespace Strem.Core.Flows;

public class TodoStore : ITodoStore
{
    public const string DefaultFlowStore = "Default";

    public List<TodoData> Todos { get; set; } = new();
}