namespace Strem.Core.Events;

public class ErrorEvent
{
    public string Source { get; }
    public string Message { get; }

    public ErrorEvent(string source, string message)
    {
        Source = source;
        Message = message;
    }
}