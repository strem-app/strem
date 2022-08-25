namespace Strem.Todos.Data;

public class TodoData
{
    public Guid Id { get; set; } = new Guid();
    public DateTime CreatedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string CreatedBy { get; set; }
    public string Title { get; set; }
    public string Payload { get; set; }
    public List<string> Tags { get; set; } = new();
    public TodoActionType ActionType { get; set; }
}