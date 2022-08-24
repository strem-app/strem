using System.Collections;

namespace Strem.Core.Components.Elements.Drag;

public class DropData
{
    public object DraggedItem { get; } 
    public IList DraggedItemList { get; }
    public object? DroppedItem { get; }
    public IList DroppedItemList { get; }
    public string DropType { get; }

    public DropData(string dropType, object draggedItem, object? droppedItem, IList draggedItemList, IList droppedItemList)
    {
        DraggedItem = draggedItem;
        DroppedItem = droppedItem;
        DraggedItemList = draggedItemList;
        DroppedItemList = droppedItemList;
        DropType = dropType;
    }
}