namespace Strem.Flows.Default.Events;

public class UserDataEvent
{
    public string EventName { get; }
    public string Data { get;  }

    public UserDataEvent(string eventName, string data)
    {
        EventName = eventName;
        Data = data;
    }
}