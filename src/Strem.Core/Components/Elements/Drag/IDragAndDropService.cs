using System.Collections;
using System.Reactive;

namespace Strem.Core.Components.Elements.Drag;

public interface IDragAndDropService
{
    IObservable<DropData> OnDroppedItem { get; }
    IObservable<Unit> OnDestinationObjectChanged { get; }
    string SourceDropType { get; }

    bool IsSourceObject(object comparisonObject);
    bool IsDestinationObject(object comparisonObject);
    
    void OnDragStart(object draggingObject, IList draggingList, string sourceDropType);
    void OnDragEnterElement(object destinationObject, IList destinationList, string destinationDropType);
    void OnDragEnd();
}