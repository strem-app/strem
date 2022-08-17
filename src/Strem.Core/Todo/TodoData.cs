using Strem.Core.Types;

namespace Strem.Core.Todo;

public class TodoData
{
    public DateTime CreatedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Title { get; set; }
    public string Payload { get; set; }
    public TodoActionType ActionType { get; set; }
}