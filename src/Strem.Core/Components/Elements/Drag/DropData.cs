namespace Strem.Core.Components.Elements.Drag;

public class DropData
{
    public object DraggedItem { get; } 
    public object? DroppedOn { get; }
    public string Context { get; }

    public DropData(object draggedItem, string context, object? droppedOn = null)
    {
        DraggedItem = draggedItem;
        DroppedOn = droppedOn;
        Context = context;
    }
}