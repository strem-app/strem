namespace Strem.Core.Events;

public class ShowNotificationEvent
{
    public string Classes { get; }
    public string Message { get; }

    public ShowNotificationEvent(string message, string classes = null)
    {
        Classes = classes;
        Message = message;
    }
}