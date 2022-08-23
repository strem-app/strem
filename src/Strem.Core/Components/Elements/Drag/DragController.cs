using System.Reactive.Subjects;

namespace Strem.Core.Components.Elements.Drag;

public class DragController
{
    public object? DraggingObject { get; private set; }
    public object? LastEnteredObject { get; private set; }
    
    private readonly Subject<DropData> _droppedItemSubject = new();
    public IObservable<DropData> OnDroppedItem => _droppedItemSubject;

    public void OnDropOverElement(object dropTarget)
    {
        LastEnteredObject=null;
        if (DraggingObject == null) return;
        if (DraggingObject == dropTarget) return;

        var dropData = new DropData(DraggingObject, "", dropTarget);
        _droppedItemSubject.OnNext(dropData);
        DraggingObject=null;
    }

    public void OnDragStart(object draggingObject)
    { DraggingObject = draggingObject; }

    public void OnDragEnterElement(object enteredItem)
    {
        if (DraggingObject == null) { return; }
        LastEnteredObject = enteredItem;
    }

    public void  OnDragEnd()
    {
        DraggingObject = null;        
        LastEnteredObject = null;
    }
}