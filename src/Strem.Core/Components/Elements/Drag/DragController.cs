using System.Collections;
using System.Reactive;
using System.Reactive.Subjects;

namespace Strem.Core.Components.Elements.Drag;

public class DragController : IDisposable
{
    public IList? DraggingList { get; private set; }
    public object? DraggingObject { get; private set; }
    public string DropType { get; private set; } = string.Empty;

    public object? LastEnteredObject { get; private set; }
    public IList? LastEnteredList { get; private set; }
    
    private readonly Subject<DropData> _droppedItemSubject = new();
    public IObservable<DropData> OnDroppedItem => _droppedItemSubject;

    public readonly Subject<Unit> _dragEnterObjectChanged = new();
    public IObservable<Unit> OnDragEnterObjectChanged => _dragEnterObjectChanged;
    
    public void OnDragStart(object draggingObject, string dropType, IList draggingList)
    {
        DraggingObject = draggingObject;
        DraggingList = draggingList;
        DropType = dropType;
    }

    public void OnDragEnterElement(object enteredObject, IList lastEnteredList)
    {
        if (DraggingObject == null) { return; }
        LastEnteredObject = enteredObject;
        LastEnteredList = lastEnteredList;
        _dragEnterObjectChanged.OnNext(Unit.Default);
    }

    public void OnDragEnd()
    {
        if (DraggingObject == null || LastEnteredObject == null)
        {
            ResetState();
            return;
        }

        var dropData = new DropData(DropType, DraggingObject, LastEnteredObject, DraggingList, LastEnteredList);
        ResetState();
        _droppedItemSubject.OnNext(dropData);
        _dragEnterObjectChanged.OnNext(Unit.Default);
    }

    public void ResetState()
    {
        DraggingObject = null;        
        DraggingList = null;        
        LastEnteredObject = null;
        DropType = string.Empty;
    }

    public void Dispose()
    {
        _droppedItemSubject.Dispose();
    }

    public bool IsDraggedObject(object comparisonObject)
    { return DraggingObject?.Equals(comparisonObject) ?? false; }
    
    public bool IsLastEnteredObject(object comparisonObject)
    { return LastEnteredObject?.Equals(comparisonObject) ?? false; }
}