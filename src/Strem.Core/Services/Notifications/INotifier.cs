namespace Strem.Core.Services.Notifications;

public interface INotifier
{
    Task ShowNotification(string title, string styles = "is-info", int showForMilliseconds = 2000);
}