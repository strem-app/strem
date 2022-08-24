using System.Collections;

namespace Strem.Core.Components.Elements.Drag;

public class DropData
{
    public object SourceObject { get; } 
    public IList? SourceList { get; }
    public string SourceDropType { get; }

    public object? DestinationObject { get; }
    public IList? DestinationList { get; }
    public string DestinationDropType { get; }

    public DropData(object sourceObject, IList? sourceList, string sourceDropType, object? destinationObject, IList? destinationList, string destinationDropType)
    {
        SourceObject = sourceObject;
        SourceList = sourceList;
        SourceDropType = sourceDropType;
        DestinationObject = destinationObject;
        DestinationList = destinationList;
        DestinationDropType = destinationDropType;
    }
}