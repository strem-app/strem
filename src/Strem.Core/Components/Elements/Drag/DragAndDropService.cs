using System.Collections;
using System.Reactive;
using System.Reactive.Subjects;

namespace Strem.Core.Components.Elements.Drag;

public class DragAndDropService : IDisposable, IDragAndDropService
{
    public IList? SourceList { get; private set; }
    public object? SourceObject { get; private set; }
    public string SourceDropType { get; private set; } = string.Empty;
    
    public IList? DestinationList { get; private set; }
    public object? DestinationObject { get; private set; }
    public string DestinationDropType { get; private set; } = string.Empty;
    
    private readonly Subject<DropData> _onDroppedItem = new();
    public IObservable<DropData> OnDroppedItem => _onDroppedItem;

    public readonly Subject<Unit> _onDestinationObjectChanged = new();
    public IObservable<Unit> OnDestinationObjectChanged => _onDestinationObjectChanged;
    
    public void OnDragStart(object draggingObject, IList draggingList, string sourceDropType)
    {
        SourceObject = draggingObject;
        SourceList = draggingList;
        SourceDropType = sourceDropType;
    }

    public void OnDragEnterElement(object destinationObject, IList destinationList, string destinationDropType)
    {
        if (SourceObject == null) { return; }
        DestinationObject = destinationObject;
        DestinationList = destinationList;
        DestinationDropType = destinationDropType;
        _onDestinationObjectChanged.OnNext(Unit.Default);
    }

    public void OnDragEnd()
    {
        if (SourceObject == null || DestinationObject == null || SourceObject.Equals(DestinationObject))
        {
            ResetState();
            _onDestinationObjectChanged.OnNext(Unit.Default);
            return;
        }

        var dropData = new DropData(SourceObject, SourceList, SourceDropType, DestinationObject, DestinationList, DestinationDropType);
        ResetState();
        _onDroppedItem.OnNext(dropData);
        _onDestinationObjectChanged.OnNext(Unit.Default);
    }

    public void ResetState()
    {
        SourceObject = null;        
        SourceList = null;        
        SourceDropType = string.Empty;        
        DestinationObject = null;
        DestinationList = null;
        DestinationDropType = string.Empty;
    }

    public void Dispose()
    {
        _onDroppedItem.Dispose();
        _onDestinationObjectChanged.Dispose();
    }

    public bool IsSourceObject(object comparisonObject)
    { return SourceObject?.Equals(comparisonObject) ?? false; }
    
    public bool IsDestinationObject(object comparisonObject)
    { return DestinationObject?.Equals(comparisonObject) ?? false; }
}