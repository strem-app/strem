namespace Strem.Core.Events;

public record ShowNotificationEvent(string Message, string Classes = null);