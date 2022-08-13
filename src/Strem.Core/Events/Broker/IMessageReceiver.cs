namespace Strem.Core.Events.Broker;

public interface IMessageReceiver
{
    /// <summary>
    /// Subscribe typed message.
    /// </summary>
    IObservable<T> Receive<T>();
}